using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using System.Data.Common;
using UserAndBankAccountServices.Data;
using UserAndBankAccountServices.Models;
using UserAndBankAccountServices.Models.Dtos;
using UserAndBankAccountServices.Repository.IRepository;

namespace UserAndBankAccountServices.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ECommerceDbContex _contex;

        public UserRepository(ECommerceDbContex contex)
        {
            _contex = contex;
        }

        public async Task<List<User>> GetAll()
        {
            return await _contex.User.Include(x => x.BankAccount).ToListAsync();
        }

        public async Task<User> GetById(string id)
        {
            return await _contex.User.Where(x => x.Id.Equals(id))
                                     .Include(x => x.BankAccount)
                                     .FirstOrDefaultAsync()
                                     ?? throw new Exception("User not found");
        }

        public async Task Post(User user)
        {
            await _contex.User.AddAsync(user);
            await _contex.SaveChangesAsync();
        }

        public async Task Update(string id, UserUpdateDto user)
        {
            var userToUpdate = await GetById(id);
            userToUpdate.Address = user.Address;

            _contex.User.Update(userToUpdate);
            await _contex.SaveChangesAsync();
        }

        public async Task SetRole(string id, string role)
        {
            var user = await GetById(id);

            user.Role = role;
            _contex.User.Update(user);
            await _contex.SaveChangesAsync();
        }

        public async Task AddBankAccountId(User user, string bankAccountId)
        {
            user.BankAccountId = bankAccountId;

            _contex.User.Update(user);
            await _contex.SaveChangesAsync();
        }

        public async Task ChangePassword(string id, byte[] passwordHash, byte[] key)
        {
            var user = await GetById(id);
            user.PasswordHash = passwordHash;
            user.Key = key;

            _contex.User.Update(user);
            await _contex.SaveChangesAsync();
        }

        public async Task Delete(string id)
        {
            var userToDelete = await GetById(id);

            _contex.User.Remove(userToDelete);
            await _contex.SaveChangesAsync();
        }

        public async Task<User> GetByEmail(string email)
        {
            return await _contex.User.Where(x => x.Email.Equals(email)).FirstOrDefaultAsync()
                ?? throw new Exception("User not found");
        }
    }
}
