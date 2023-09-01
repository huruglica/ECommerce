using MongoDB.Driver;
using ProductAndOrderServices.Data;
using ProductAndOrderServices.Model;
using ProductAndOrderServices.Model.Dtos;
using ProductAndOrderServices.Repository.IRepository;

namespace ProductAndOrderServices.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ECommerceDbContext _dbContext;

        public OrderRepository(ECommerceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IClientSessionHandle> StartSession()
        {
            return await _dbContext.StarSession();
        }

        public async Task<List<Order>> GetAll()
        {
            return await _dbContext.Order.Find(_ => true).ToListAsync();
        }

        public async Task<List<Order>> GetMyOrders(string userId)
        {
            var filter = Builders<Order>.Filter.Eq("UserId", userId);
            return await _dbContext.Order.Find(filter).ToListAsync();
        }

        public async Task<Order> GetById(string id)
        {
            var filter = Builders<Order>.Filter.Eq("Id", id);
            return await _dbContext.Order.Find(filter).FirstOrDefaultAsync()
                         ?? throw new Exception("Order not found");
        }

        public async Task Post(Order order)
        {
            await _dbContext.Order.InsertOneAsync(order);
        }

        public async Task<UpdateResult> Update(string id, OrderUpdateDto order)
        {
            var filter = Builders<Order>.Filter.Eq("Id", id);
            var update = Builders<Order>.Update.Set(x => x.Address, order.Address);

            return await _dbContext.Order.UpdateOneAsync(filter, update);
        }

        public async Task<UpdateResult> RemoveProduct(string id, double price, List<ProductSimple> products)
        {
            var filter = Builders<Order>.Filter.Eq("Id", id);

            var update = Builders<Order>.Update
                .Set(x => x.Products, products)
                .Set(x => x.Price, price);

            return await _dbContext.Order.UpdateOneAsync(filter, update);
        }

        public async Task<UpdateResult> AddProduct(string id, double price, List<ProductSimple> products)
        {
            var filter = Builders<Order>.Filter.Eq("Id", id);

            var update = Builders<Order>.Update
                .Set(x => x.Products, products)
                .Set(x => x.Price, price);

            return await _dbContext.Order.UpdateOneAsync(filter, update);
        }

        public async Task<DeleteResult> Delete(string id)
        {
            var filter = Builders<Order>.Filter.Eq("Id", id);
            return await _dbContext.Order.DeleteOneAsync(filter);
        }

        public async Task<UpdateResult> BuyOrder(IClientSessionHandle session, string id)
        {
            var filter = Builders<Order>.Filter.Eq("Id", id);
            var update = Builders<Order>.Update
                .Set(x => x.IsBought, true)
                .Set(x => x.BoughtTime, DateTime.Now);
            
            return await _dbContext.Order.UpdateOneAsync(session, filter, update);
        }

        public async Task<UpdateResult> ReturnOrder(IClientSessionHandle session, string id)
        {
            var filter = Builders<Order>.Filter.Eq("Id", id);
            var update = Builders<Order>.Update.Set(x => x.IsBought, false);

            return await _dbContext.Order.UpdateOneAsync(session, filter, update);
        }
    }
}
