namespace ProductAndOrderServices.Model.Dtos
{
    public class ProductUpdateDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? Discount { get; set; }
    }
}
