using UserAndBankAccountServices.Model.Dtos;
using Microsoft.EntityFrameworkCore.Storage;
using UserAndBankAccountServices.Models;

namespace UserAndBankAccountServices.Repository.IRepository
{
    public interface IBankAccountRepository
    {
        Task<IDbContextTransaction> BeginTransaction();
        Task<List<BankAccount>> GetAll();
        Task<BankAccount> GetById(string id);
        Task Post(BankAccount bankAccount);
        Task Update(string id, BankAccountDto bankAccount);
        Task Delete(string id);
        Task Deposite(string id, BankAccountDto bankAccount);
        Task Withdraw(string id, BankAccountDto bankAccount);
    }
}
