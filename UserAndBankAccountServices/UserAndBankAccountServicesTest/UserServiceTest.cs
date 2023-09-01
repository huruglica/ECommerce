using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Security.Cryptography;
using UserAndBankAccountServices.Controllers;
using UserAndBankAccountServices.Data;
using UserAndBankAccountServices.Models;
using UserAndBankAccountServices.Models.Dtos;
using UserAndBankAccountServices.Repository;
using UserAndBankAccountServicesTest.Helpers;

namespace UserAndBankAccountServicesTest
{
    public class UserServiceTest : IClassFixture<UserCreateDtoValidatorFixture>,
                                   IClassFixture<UserUpdateDtoValidatorFixture>,
                                   IClassFixture<AutoMapperFixture>
    {
        private readonly UserAndBankAccountServices.Service.UserService _userService;
        private readonly UserController _userController;

        private readonly UserCreateDtoValidatorFixture _userCreateDtoValidatorFixture;
        private readonly UserUpdateDtoValidatorFixture _userUpdateDtoValidatorFixture;
        private readonly AutoMapperFixture _autoMapperFixture;

        private readonly HttpContextAccessorHelper _httpContextAccessorHelper;

        private readonly ECommerceDbContex _eCommerceDbContex;

        private readonly UserRepository _userRepository;
        private readonly IValidator<UserCreateDto> _userCreateValidator;
        private readonly IValidator<UserUpdateDto> _userUpdateValidator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public UserServiceTest(UserCreateDtoValidatorFixture userCreateDtoValidatorFixture, UserUpdateDtoValidatorFixture userUpdateDtoValidatorFixture, AutoMapperFixture autoMapperFixture)
        {
            DbContextOptionsBuilder<ECommerceDbContex> dbOptions = new DbContextOptionsBuilder<ECommerceDbContex>()
                        .UseInMemoryDatabase(Guid.NewGuid().ToString());

            _eCommerceDbContex = new ECommerceDbContex(dbOptions.Options);
            _userRepository = new UserRepository(_eCommerceDbContex);

            _userCreateDtoValidatorFixture = userCreateDtoValidatorFixture;
            _userUpdateDtoValidatorFixture = userUpdateDtoValidatorFixture;
            _autoMapperFixture = autoMapperFixture;

            _userCreateValidator = _userCreateDtoValidatorFixture.ServiceProvider.GetRequiredService<IValidator<UserCreateDto>>();
            _userUpdateValidator = _userUpdateDtoValidatorFixture.ServiceProvider.GetRequiredService<IValidator<UserUpdateDto>>();
            _mapper = _autoMapperFixture.ServiceProvider.GetRequiredService<IMapper>();

            _httpContextAccessorHelper = new HttpContextAccessorHelper();
            _httpContextAccessor = _httpContextAccessorHelper.HttpContextAccessor;

            _userService = new UserAndBankAccountServices.Service.UserService(_userRepository,
                               _userCreateValidator,
                               _userUpdateValidator,
                               _mapper,
                               _httpContextAccessor);
            _userController = new UserController(_userService);
        }

        [Fact]
        public async Task Test_GetAll()
        {
            var userOne = new User
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Name",
                Surname = "Surname",
                Address = "Address",
                Email = "email@email.ee",
                PasswordHash = new byte[10],
                Key = new byte[10],
                Role = "User",
                BankAccountId = Guid.NewGuid().ToString()
            };

            var userTwo = new User
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Name",
                Surname = "Surname",
                Address = "Address",
                Email = "email@email.ee",
                PasswordHash = new byte[10],
                Key = new byte[10],
                Role = "User",
                BankAccountId = Guid.NewGuid().ToString()
            };

            await _userRepository.Post(userOne);
            await _userRepository.Post(userTwo);

            var result = await _userController.GetAll();
            Assert.IsType<OkObjectResult>(result);

            var users = await _userRepository.GetAll();
            Assert.Equal(2, users.Count);
        }

        [Theory]
        [InlineData("64f1bd63826610e30d527560", "64f1bd63826610e30d527561")]
        public async Task Test_GetById(string okId, string notOkId)
        {
            var user = new User
            {
                Id = okId,
                Name = "Name",
                Surname = "Surname",
                Address = "Address",
                Email = "email@email.ee",
                PasswordHash = new byte[10],
                Key = new byte[10],
                Role = "User",
                BankAccountId = Guid.NewGuid().ToString()
            };

            await _userRepository.Post(user);

            var result = await _userController.GetById(okId);
            var notOkResult = await _userController.GetById(notOkId);

            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<BadRequestObjectResult>(notOkResult);
        }

        [Theory]
        [InlineData("Name", "Surname", "Address", "email@email.ee", "Password")]
        public async Task Test_PostOk(string name, string surname, string address, string email, string password)
        {
            var userCreateDto = new UserCreateDto
            {
                Name = name,
                Surname = surname,
                Address = address,
                Email = email,
                Password = password
            };

            var result = await _userController.Post(userCreateDto);
            Assert.IsType<OkObjectResult>(result);
        }

        [Theory]
        [InlineData("", "Surname", "Address", "email@email.ee", "Password")]
        [InlineData(null, "Surname", "Address", "email@email.ee", "Password")]
        [InlineData("Name", "", "Address", "email@email.ee", "Password")]
        [InlineData("Name", null, "Address", "email@email.ee", "Password")]
        [InlineData("Name", "Surname", "", "email@email.ee", "Password")]
        [InlineData("Name", "Surname", null, "email@email.ee", "Password")]
        [InlineData("Name", "Surname", "Address", "", "Password")]
        [InlineData("Name", "Surname", "Address", null, "Password")]
        [InlineData("Name", "Surname", "Address", "email@email", "Password")]
        [InlineData("Name", "Surname", "Address", "email@.ee", "Password")]
        [InlineData("Name", "Surname", "Address", "email@.email.ee", "Password")]
        [InlineData("Name", "Surname", "Address", "@email.ee", "Password")]
        [InlineData("Name", "Surname", "Address", "email@email.ee", "")]
        [InlineData("Name", "Surname", "Address", "email@email.ee", null)]
        [InlineData("Name", "Surname", "Address", "email@email.ee", "Pass")]
        public async Task Test_PostNotOk(string name, string surname, string address, string email, string password)
        {
            var userCreateDto = new UserCreateDto
            {
                Name = name,
                Surname = surname,
                Address = address,
                Email = email,
                Password = password
            };

            var result = await _userController.Post(userCreateDto);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Theory]
        [InlineData("SuperAdmin")]
        [InlineData("Admin")]
        [InlineData("User")]
        public async Task Test_SetRoleOk(string role)
        {
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Name",
                Surname = "Surname",
                Address = "Address",
                Email = "email@email.ee",
                PasswordHash = new byte[10],
                Key = new byte[10],
                Role = "User",
                BankAccountId = Guid.NewGuid().ToString()
            };

            await _userRepository.Post(user);

            var result = await _userController.SetRole(user.Id, role);
            Assert.IsType<OkObjectResult>(result);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("Supervisor")]
        [InlineData("Regular")]
        [InlineData("Manager")]
        public async Task Test_SetRoleNotOk(string role)
        {
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Name",
                Surname = "Surname",
                Address = "Address",
                Email = "email@email.ee",
                PasswordHash = new byte[10],
                Key = new byte[10],
                Role = "User",
                BankAccountId = Guid.NewGuid().ToString()
            };

            await _userRepository.Post(user);

            var result = await _userController.SetRole(user.Id, role);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Theory]
        [InlineData("64f1bd63826610e30d527560", "64f1bd63826610e30d527561")]
        public async Task Test_Delete(string okId, string notOkId)
        {
            var user = new User
            {
                Id = okId,
                Name = "Name",
                Surname = "Surname",
                Address = "Address",
                Email = "email@email.ee",
                PasswordHash = new byte[10],
                Key = new byte[10],
                Role = "User",
                BankAccountId = Guid.NewGuid().ToString()
            };

            await _userRepository.Post(user);

            var result = await _userController.Delete(okId);
            var notOkResult = await _userController.Delete(notOkId);

            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<BadRequestObjectResult>(notOkResult);
        }

        [Theory]
        [InlineData("NewAddress")]
        public async Task Test_UpdateOk(string address)
        {
            var user = new User
            {
                Id = "64dcd34fe55c1e2ee8460991",
                Name = "Name",
                Surname = "Surname",
                Address = "Address",
                Email = "email@email.ee",
                PasswordHash = new byte[10],
                Key = new byte[10],
                Role = "User",
                BankAccountId = Guid.NewGuid().ToString()
            };

            await _userRepository.Post(user);

            var userUpdateDto = new UserUpdateDto
            {
                Address = address
            };

            var result = await _userController.Update(userUpdateDto);
            Assert.IsType<OkObjectResult>(result);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("AddressAddressAddressAddressAddressAddressAddressAddressAddress")]
        public async Task Test_UpdateNotOk(string address)
        {
            var user = new User
            {
                Id = "64dcd34fe55c1e2ee8460991",
                Name = "Name",
                Surname = "Surname",
                Address = "Address",
                Email = "email@email.ee",
                PasswordHash = new byte[10],
                Key = new byte[10],
                Role = "User",
                BankAccountId = Guid.NewGuid().ToString()
            };

            await _userRepository.Post(user);

            var userUpdateDto = new UserUpdateDto
            {
                Address = address
            };

            var result = await _userController.Update(userUpdateDto);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Test_DeleteMyAccountOk()
        {
            var user = new User
            {
                Id = "64dcd34fe55c1e2ee8460991",
                Name = "Name",
                Surname = "Surname",
                Address = "Address",
                Email = "email@email.ee",
                PasswordHash = new byte[10],
                Key = new byte[10],
                Role = "User",
                BankAccountId = Guid.NewGuid().ToString()
            };

            await _userRepository.Post(user);

            var result = await _userController.Delete();
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Test_DeleteMyAccountNotOk()
        {
            var user = new User
            {
                Id = "64dcd34fe55c1e2ee8460992",
                Name = "Name",
                Surname = "Surname",
                Address = "Address",
                Email = "email@email.ee",
                PasswordHash = new byte[10],
                Key = new byte[10],
                Role = "User",
                BankAccountId = Guid.NewGuid().ToString()
            };

            await _userRepository.Post(user);

            var result = await _userController.Delete();
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Theory]
        [InlineData("NewPassword")]
        public async Task Test_ChangePasswordOk(string password)
        {
            var user = new User
            {
                Id = "64dcd34fe55c1e2ee8460991",
                Name = "Name",
                Surname = "Surname",
                Address = "Address",
                Email = "email@email.ee",
                PasswordHash = new byte[10],
                Key = new byte[10],
                Role = "User",
                BankAccountId = Guid.NewGuid().ToString()
            };

            await _userRepository.Post(user);

            var result = await _userController.ChangePassword(password);
            Assert.IsType<OkObjectResult>(result);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("Pass")]
        public async Task Test_ChangePasswordNotOk(string password)
        {
            var user = new User
            {
                Id = "64dcd34fe55c1e2ee8460991",
                Name = "Name",
                Surname = "Surname",
                Address = "Address",
                Email = "email@email.ee",
                PasswordHash = new byte[10],
                Key = new byte[10],
                Role = "User",
                BankAccountId = Guid.NewGuid().ToString()
            };

            await _userRepository.Post(user);

            var result = await _userController.ChangePassword(password);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Theory]
        [InlineData("email@email.ee", "Password")]
        public async Task Test_LoginOk(string email, string password)
        {
            var userCreateDto = new UserCreateDto
            {
                Name = "Name",
                Surname = "Surname",
                Address = "Address",
                Email = "email@email.ee",
                Password = password
            };

            await _userService.Post(userCreateDto);

            var userLogin = new UserLogin
            {
                Email = email,
                Password = password
            };

            var result = await _userController.Login(userLogin);
            Assert.IsType<OkObjectResult>(result);
        }

        [Theory]
        [InlineData("email@email.ee", "password")]
        [InlineData("email@email.de", "Password")]
        public async Task Test_LoginNotOk(string email, string password)
        {
            var userCreateDto = new UserCreateDto
            {
                Name = "Name",
                Surname = "Surname",
                Address = "Address",
                Email = "email@email.ee",
                Password = "Password"
            };

            await _userService.Post(userCreateDto);

            var userLogin = new UserLogin
            {
                Email = email,
                Password = password
            };

            var result = await _userController.Login(userLogin);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Test_GetMyDataOk()
        {
            var user = new User
            {
                Id = "64dcd34fe55c1e2ee8460991",
                Name = "Name",
                Surname = "Surname",
                Address = "Address",
                Email = "email@email.ee",
                PasswordHash = new byte[10],
                Key = new byte[10],
                Role = "User",
                BankAccountId = Guid.NewGuid().ToString()
            };

            await _userRepository.Post(user);

            var result = await _userController.GetMyData();
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Test_GetMyDataNotOk()
        {
            var user = new User
            {
                Id = "64dcd34fe55c1e2ee8460992",
                Name = "Name",
                Surname = "Surname",
                Address = "Address",
                Email = "email@email.ee",
                PasswordHash = new byte[10],
                Key = new byte[10],
                Role = "User",
                BankAccountId = Guid.NewGuid().ToString()
            };

            await _userRepository.Post(user);

            var result = await _userController.GetMyData();
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
