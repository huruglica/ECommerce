using AutoMapper;
using FluentValidation;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UserAndBankAccountServices.Helpers;
using UserAndBankAccountServices.Models;
using UserAndBankAccountServices.Models.Dtos;
using UserAndBankAccountServices.Repository.IRepository;
using UserAndBankAccountServices.Services.IServices;
using UserService;
using static UserService.UserService;

namespace UserAndBankAccountServices.Service
{
    public class UserService : UserServiceBase, IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IValidator<UserCreateDto> _userCreateValidator;
        private readonly IValidator<UserUpdateDto> _userUpdateValidator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private const string secretKey = "3RiMMI3eusj2CJu15cJQIpXP8YallpXQQj8ad_13GiLu4uS7sUxL3Wezw6HpzfLL";
        public UserService(IUserRepository userRepository, IValidator<UserCreateDto> userCreateValidator, IValidator<UserUpdateDto> userUpdateValidator, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _userCreateValidator = userCreateValidator;
            _userUpdateValidator = userUpdateValidator;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<UserDto>> GetAll()
        {
            var users = await _userRepository.GetAll();
            return _mapper.Map<List<UserDto>>(users);
        }

        public async Task<UserDto> GetById(string id)
        {
            var user = await _userRepository.GetById(id);
            return _mapper.Map<UserDto>(user);
        }

        private async Task<User> GetUserById(string id)
        {
            return await _userRepository.GetById(id);
        }

        #region POST
        public async Task Post(UserCreateDto user)
        {
            var validator = _userCreateValidator.Validate(user);

            if (!validator.IsValid)
            {
                throw new Exception(validator.ToString());
            }

            var userToAdd = _mapper.Map<User>(user);
            userToAdd.Id = ObjectId.GenerateNewId().ToString();
            GenerateHash(user.Password, out byte[] passwordHash, out byte[] key);
            userToAdd.PasswordHash = passwordHash;
            userToAdd.Key = key;
            userToAdd.Role = "User";

            await _userRepository.Post(userToAdd);
        }

        private void GenerateHash(string password, out byte[] passwordHash, out byte[] key)
        {
            var hmac = new HMACSHA256();

            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            key = hmac.Key;
        }
        #endregion

        public async Task Update(string id, UserUpdateDto user)
        {
            var validator = _userUpdateValidator.Validate(user);

            if (!validator.IsValid)
            {
                throw new Exception(validator.ToString());
            }

            await _userRepository.Update(id, user);
        }

        public async Task SetRole(string id, string role)
        {
            if (!Enum.IsDefined(typeof(Roles), role))
            {
                throw new Exception("This role is not available");
            }

            await _userRepository.SetRole(id, role);
        }

        public async Task AddBankAccountId(string id, string bankAccountId)
        {
            var user = await GetUserById(id);

            if (user.BankAccountId != null)
            {
                throw new Exception("You already have an bank account");
            }

            await _userRepository.AddBankAccountId(user, bankAccountId);
        }

        public async Task Delete(string id)
        {
            await _userRepository.Delete(id);
        }

        #region LOGIN
        public async Task<string> Login(UserLogin userLogin)
        {
            var user = await _userRepository.GetByEmail(userLogin.Email);
            
            if (VerifyPassword(userLogin.Password, user.PasswordHash, user.Key))
            {
                return GenerateToken(user);
            }

            throw new Exception("Wrong password");
        }

        private bool VerifyPassword(string password, byte[] passwordHash, byte[] key)
        {
            var hmac = new HMACSHA256(key);

            var passwordHashToVerify = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            return passwordHashToVerify.SequenceEqual(passwordHash);
        }

        private string GenerateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("BankAccountId", user.BankAccountId ?? ""),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: "dev-nq3upfdndrxpn4bz.us.auth0.com",
                audience: "ECommerce",
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        #endregion

        public async Task<UserDto> GetMyData()
        {
            var id = await GetUserId();

            var user = await _userRepository.GetById(id);
            
            return _mapper.Map<UserDto>(user);
        }

        public async Task Update(UserUpdateDto user)
        {
            var id = await GetUserId();
            await Update(id, user);
        }

        public async Task ChangePassword(string newPassword)
        {
            if (newPassword.Length < 8)
            {
                throw new Exception("Password is to short");
            }

            var id = await GetUserId();

            GenerateHash(newPassword, out byte[] passwordHash, out byte[] key);

            await _userRepository.ChangePassword(id, passwordHash, key);
        }

        public async Task Delete()
        {
            var id = await GetUserId();
            await Delete(id);
        }

        private async Task<string> GetUserId()
        {
            return await Task.Run(() => _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x =>
                         x.Type == ClaimTypes.NameIdentifier)?.Value
                         ?? throw new Exception("You must login first"));
        }

        public override async Task<UserInfoResponse> GetUserInfo(UserIdRequest request, ServerCallContext context)
        {
            var user = await GetById(request.UserId);

            var response = new UserInfoResponse
            {
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                BankAccountId = user.BankAccountId
            };

            return response;
        }

        public override async Task<BankAccountIdResponse> GetBankAccountId(UserIdRequest request, ServerCallContext context)
        {
            var user = await GetById(request.UserId);
            var response = new BankAccountIdResponse();
            response.BankAccountId = user.BankAccountId;
            return response;
        }
    }
}
