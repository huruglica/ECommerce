using MongoDB.Driver;
using ProductAndOrderServices.Model;

namespace ProductAndOrderServices.Data
{
    public class ECommerceDbContext
    {
        private readonly IMongoDatabase? _mongoDatabase;
        private MongoClient _client = new MongoClient("mongodb://localhost:27017");

        public ECommerceDbContext(MongoClient client)
        {
            _mongoDatabase = client.GetDatabase("ECommerce");
        }

        public async Task<IClientSessionHandle> StarSession()
        {
            return await _client.StartSessionAsync();
        }

        public IMongoCollection<Order> Order => _mongoDatabase?.GetCollection<Order>("Order")
                                             ?? throw new Exception("Problem loading data base");
        public IMongoCollection<Product> Product => _mongoDatabase?.GetCollection<Product>("Product")
                                                 ?? throw new Exception("Problem loading data base");
    }
}
