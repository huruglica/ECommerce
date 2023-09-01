using ProductAndOrderServices.Model.Dtos;
using ProductAndOrderServices.Model;
using MongoDB.Driver;
using ProductAndOrderServices.Helpers;

namespace ProductAndOrderServices.Repository.IRepository
{
    public interface IProductRepository
    {
        Task<PagedInfo<Product>> GetAll(SearchAndSort searchAndSort);
        Task<List<Product>> GetMyProducts(string userId);
        Task<Product> GetById(string id);
        Task Post(Product product);
        Task<Product> Update(string id, ProductUpdateDto product);
        Task UpdateStock(string id, int stock);
        Task<Product> UpdateStock(IClientSessionHandle session, string id, int stock);
        Task<DeleteResult> Delete(string id);
        Task<List<Product>> Get(List<string> filters);
    }
}
