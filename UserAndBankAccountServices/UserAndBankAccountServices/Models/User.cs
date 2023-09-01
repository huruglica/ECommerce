using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserAndBankAccountServices.Models
{
    public class User
    {
        public string Id { get; set; }
        [Column(TypeName = "varchar")]
        [MaxLength(15)]
        public string Name { get; set; }
        [Column(TypeName = "varchar")]
        [MaxLength(15)]
        public string Surname { get; set; }
        [Column(TypeName = "varchar")]
        [MaxLength(60)]
        public string Address { get; set; }
        [Column(TypeName = "varchar")]
        [MaxLength(50)]
        public string Email { get; set; }
        public byte[] Key { get; set; }
        public byte[] PasswordHash { get; set; }
        [Column(TypeName = "varchar")]
        [MaxLength(10)]
        public string Role { get; set; }
        public string? BankAccountId { get; set; }

        public virtual BankAccount? BankAccount { get; set; }
    }
}
