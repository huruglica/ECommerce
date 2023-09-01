using MongoDB.Driver;
using ProductAndOrderServices.Helpers;
using ProductAndOrderServices.Model;
using ProductAndOrderServices.Model.Dtos;

namespace ProductAndOrderServices.Services.IServices
{
    public interface IProductService
    {
        Task<PagedInfo<Product>> GetAll(SearchAndSort searchAndSort);
        Task<Product> GetById(string id);
        Task Post(ProductCreateDto product);
        Task Update(string id, ProductUpdateDto product);
        Task Delete(string id);

        Task<ProductSimple> GetProduct(ProductSimpleCreateDto productSimpleCreateDto);
        Task<List<TransferInfo>> UpdateStockAndTakeSellersInfo(IClientSessionHandle session, bool isBuy, List<ProductSimple> productsSimple);
        Task<List<Product>> GetMyProducts();
        Task UpdateMyProduct(string id, ProductUpdateDto product);
        Task DeleteMyProduct(string id);
    }
}
