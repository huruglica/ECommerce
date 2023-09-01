using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ProductAndOrderServices.Model.Dtos
{
    public class ProductSimpleCreateDto
    {
        [Required]
        public string Id { get; set; }
        [Range(1, 100)]
        [DefaultValue(1)]
        public int Quantity { get; set; } = 1;
    }
}
