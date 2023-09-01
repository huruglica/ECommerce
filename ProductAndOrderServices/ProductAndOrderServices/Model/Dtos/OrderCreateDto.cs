namespace ProductAndOrderServices.Model.Dtos
{
    public class OrderCreateDto
    {
        public string Address { get; set; }
        public List<ProductSimpleCreateDto> ProductsCreateDto { get; set; }
    }
}
