using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProductAndOrderServices.Model
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Address { get; set; }
        public double Price { get; set; }
        public string UserId { get; set; }
        public bool IsBought { get; set; } = false;
        public DateTime BoughtTime { get; set; }
        public List<ProductSimple> Products { get; set; }
    }
}
