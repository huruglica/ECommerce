using ProductAndOrderServices.Model;
using ProductAndOrderServices.Model.Dtos;

namespace ProductAndOrderServices.Services.IServices
{
    public interface IOrderService
    {
        Task<List<Order>> GetAll();
        Task<Order> GetById(string id);
        Task Post(OrderCreateDto order);
        Task Update(string id, OrderUpdateDto order);
        Task Delete(string id);

        Task BuyOrder(string id);
        Task ReturnOrder(string id);
        Task<List<Order>> GetMyOrders();
        Task<Order> GetMyOrder(string id);
        Task UpdateMyOrder(string id, OrderUpdateDto order);
        Task RemoveProduct(string id, ProductSimpleCreateDto productSimple);
        Task AddProduct(string id, ProductSimpleCreateDto productSimple);
        Task DeleteMyOrder(string id);
    }
}
