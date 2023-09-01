using Microsoft.EntityFrameworkCore;
using UserAndBankAccountServices.Models;

namespace UserAndBankAccountServices.Data
{
    public class ECommerceDbContex : DbContext
    {
        public ECommerceDbContex(DbContextOptions<ECommerceDbContex> options) : base(options)
        {

        }

        public DbSet<User> User { get; set; }
        public DbSet<BankAccount> BankAccount { get; set; }
    }
}
