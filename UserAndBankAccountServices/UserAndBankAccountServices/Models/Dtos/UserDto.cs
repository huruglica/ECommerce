using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UserAndBankAccountServices.Models.Dtos
{
    public class UserDto
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
        [Column(TypeName = "varchar")]
        [MaxLength(10)]
        public string Role { get; set; }
        public string? BankAccountId { get; set; }

        public virtual BankAccount? BankAccount { get; set; }
    }
}
