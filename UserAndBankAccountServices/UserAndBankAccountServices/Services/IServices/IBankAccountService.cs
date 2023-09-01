using UserAndBankAccountServices.Models;
using UserAndBankAccountServices.Model.Dtos;

namespace UserAndBankAccountServices.Services.IServices
{
    public interface IBankAccountService
    {
        Task<List<BankAccount>> GetAll();
        Task<BankAccount> GetById(string id);
        Task<BankAccount> GetMyBankAccount();
        Task Post(BankAccountDto bankAccount);
        Task Update(string id, BankAccountDto bankAccount);
        Task Delete(string id);
        Task Deposite(double amount);
        Task Withdraw(double amount);
    }
}
