namespace ProductAndOrderServices.Model.Dtos
{
    public class ProductCreateDto
    {
        public string Name { get; set; }
        public int Stock { get; set; }
        public double BasePrice { get; set; }
        public string Description { get; set; }
        public Dictionary<string, string> Specificities { get; set; }
    }
}
