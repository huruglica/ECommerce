using UserAndBankAccountServices.Models.Dtos;
using UserAndBankAccountServices.Models;

namespace UserAndBankAccountServices.Services.IServices
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAll();
        Task<UserDto> GetById(string id);
        Task Post(UserCreateDto user);
        Task Update(string id, UserUpdateDto user);
        Task SetRole(string id, string role);
        Task AddBankAccountId(string id, string bankAccountId);
        Task Delete(string id);
        Task<string> Login(UserLogin userLogin);
        Task<UserDto> GetMyData();
        Task Update(UserUpdateDto user);
        Task ChangePassword(string newPassword);
        Task Delete();
    }
}
