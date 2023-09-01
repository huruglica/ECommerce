using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ProductAndOrderServices.Model
{
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public int Stock { get; set; }
        public double BasePrice { get; set; }
        public int Discount { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public string SellerId { get; set; }
        public Dictionary<string, string> Specificities { get; set; }
    }
}
