using MongoDB.Driver;
using ProductAndOrderServices.Model;
using ProductAndOrderServices.Model.Dtos;

namespace ProductAndOrderServices.Repository.IRepository
{
    public interface IOrderRepository
    {
        Task<IClientSessionHandle> StartSession();
        Task<List<Order>> GetAll();
        Task<List<Order>> GetMyOrders(string userId);
        Task<Order> GetById(string id);
        Task Post(Order order);
        Task<UpdateResult> BuyOrder(IClientSessionHandle session, string id);
        Task<UpdateResult> ReturnOrder(IClientSessionHandle session, string id);
        Task<UpdateResult> Update(string id, OrderUpdateDto order);
        Task<UpdateResult> RemoveProduct(string id, double price, List<ProductSimple> products);
        Task<UpdateResult> AddProduct(string id, double price, List<ProductSimple> products);
        Task<DeleteResult> Delete(string id);
    }
}
