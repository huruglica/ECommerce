using UserAndBankAccountServices.Data;
using UserAndBankAccountServices.Models;
using UserAndBankAccountServices.Model.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using UserAndBankAccountServices.Repository.IRepository;

namespace UserAndBankAccountServices.Repository
{
    public class BankAccountRepository : IBankAccountRepository
    {
        private readonly ECommerceDbContex _context;

        public BankAccountRepository(ECommerceDbContex context)
        {
            _context = context;
        }

        public async Task<IDbContextTransaction> BeginTransaction()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public async Task<List<BankAccount>> GetAll()
        {
            return await _context.BankAccount.ToListAsync();
        }

        public async Task<BankAccount> GetById(string id)
        {
            return await _context.BankAccount.Where(x => x.Id.Equals(id)).FirstOrDefaultAsync()
                ?? throw new Exception("Bank Account not found");
        }

        public async Task Post(BankAccount bankAccount)
        {
            await _context.BankAccount.AddAsync(bankAccount);

            await _context.SaveChangesAsync();
        }

        public async Task Update(string id, BankAccountDto bankAccount)
        {
            var bankAccountToUpdate = await GetById(id);
            bankAccountToUpdate.Amount = bankAccount.Amount;

            _context.BankAccount.Update(bankAccountToUpdate);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(string id)
        {
            var bankAccountToDelete = await GetById(id);

            _context.BankAccount.Remove(bankAccountToDelete);
            await _context.SaveChangesAsync();
        }

        public async Task Deposite(string id, BankAccountDto bankAccount)
        {
            await Update(id, bankAccount);
        }

        public async Task Withdraw(string id, BankAccountDto bankAccount)
        {
            await Update(id, bankAccount);
        }
    }
}
