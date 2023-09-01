using System.Data.Common;
using UserAndBankAccountServices.Models;
using UserAndBankAccountServices.Models.Dtos;

namespace UserAndBankAccountServices.Repository.IRepository
{
    public interface IUserRepository
    {
        Task<List<User>> GetAll();
        Task<User> GetById(string id);
        Task Post(User user);
        Task Update(string id, UserUpdateDto user);
        Task SetRole(string id, string role);
        Task AddBankAccountId(User user, string bankAccountId);
        Task ChangePassword(string id, byte[] passwordHash, byte[] key);
        Task Delete(string id);
        Task<User> GetByEmail(string email);
    }
}
