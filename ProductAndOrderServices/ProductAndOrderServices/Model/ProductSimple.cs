using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProductAndOrderServices.Model
{
    public class ProductSimple
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
    }
}
