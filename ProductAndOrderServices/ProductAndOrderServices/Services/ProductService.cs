using AutoMapper;
using FluentValidation;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using ProductAndOrderServices.ElasasticSearch;
using ProductAndOrderServices.Helpers;
using ProductAndOrderServices.Model;
using ProductAndOrderServices.Model.Dtos;
using ProductAndOrderServices.Repository.IRepository;
using ProductAndOrderServices.Services.IServices;
using System.Security.Claims;

namespace ProductAndOrderServices.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IValidator<ProductCreateDto> _createDtoValidator;
        private readonly IValidator<ProductUpdateDto> _updateDtoValidator;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ElasticSearch _elasticSearch;

        public ProductService(IProductRepository productRepository, IValidator<ProductCreateDto> createDtoValidator, IValidator<ProductUpdateDto> updateDtoValidator, IMapper mapper, IHttpContextAccessor httpContextAccessor, ElasticSearch elasticSearch)
        {
            _productRepository = productRepository;
            _createDtoValidator = createDtoValidator;
            _updateDtoValidator = updateDtoValidator;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _elasticSearch = elasticSearch;
        }

        public async Task<PagedInfo<Product>> GetAll(SearchAndSort searchAndSort)
        {
            return await _elasticSearch.GetAll(searchAndSort);
        }

        public async Task<Product> GetById(string id)
        {
            return await _productRepository.GetById(id);
        }

        public async Task Post(ProductCreateDto product)
        {
            await ValidatePruductAndUser(product);
            var productToPost = _mapper.Map<Product>(product);
            productToPost.SellerId = await GetUserId();
            productToPost.Price = product.BasePrice;

            await _productRepository.Post(productToPost);

            await _elasticSearch.Insert(productToPost);
        }

        private async Task ValidatePruductAndUser(ProductCreateDto product)
        {
            var validator = await Task.Run(() => _createDtoValidator.Validate(product));

            if (!validator.IsValid)
            {
                throw new Exception(validator.ToString());
            }

            var userBankAccountId = await Task.Run(() =>
                   _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x =>
                   x.Type == "BankAccountId")?.Value);

            if (userBankAccountId.IsNullOrEmpty())
            {
                throw new Exception("You must first add a bank account");
            }
        }

        public async Task Update(string id, ProductUpdateDto product)
        {
            var validator = _updateDtoValidator.Validate(product);

            if (!validator.IsValid)
            {
                throw new Exception(validator.ToString());
            }

            var updatedProduct = await _productRepository.Update(id, product);

            await _elasticSearch.Update(id, updatedProduct);
        }

        public async Task Delete(string id)
        {
            var deleteResult = await _productRepository.Delete(id);

            if (deleteResult.DeletedCount <= 0)
            {
                throw new Exception("Not deleted");
            }

            await _elasticSearch.Delete(id);
        }

        public async Task<List<Product>> GetMyProducts()
        {
            var userId = await GetUserId();
            return await _productRepository.GetMyProducts(userId);
        }

        public async Task UpdateMyProduct(string id, ProductUpdateDto product)
        {
            var validator = _updateDtoValidator.Validate(product);

            if (!validator.IsValid)
            {
                throw new Exception(validator.ToString());
            }

            var userId = await GetUserId();
            var productToUpdate = await GetById(id);

            if (!productToUpdate.SellerId.Equals(userId))
            {
                throw new Exception("This is not your product");
            }

            await Update(id, product);
        }

        public async Task DeleteMyProduct(string id)
        {
            var userId = await GetUserId();
            var productToDelete = await GetById(id);

            if (!productToDelete.SellerId.Equals(userId))
            {
                throw new Exception("This is not your product");
            }

            await Delete(id);
        }

        private async Task<string> GetUserId()
        {
            return await Task.Run(() => _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x =>
                   x.Type == ClaimTypes.NameIdentifier)?.Value
                   ?? throw new Exception("You must login first"));
        }

        public async Task<ProductSimple> GetProduct(ProductSimpleCreateDto productSimpleCreateDto)
        {
            var productSimple = new ProductSimple();
            var product = await GetById(productSimpleCreateDto.Id);

            if (product.Stock <= 0)
            {
                return productSimple;
            }

            if (product.Stock < productSimpleCreateDto.Quantity)
            {
                productSimple.Id = product.Id;
                productSimple.Name = product.Name;
                productSimple.Price = product.Price; 
                productSimple.Quantity = product.Stock;
            }
            else
            {
                productSimple.Id = product.Id;
                productSimple.Name = product.Name;
                productSimple.Price = product.Price;
                productSimple.Quantity = productSimpleCreateDto.Quantity;
            }

            return productSimple;
        }

        public async Task<List<TransferInfo>> UpdateStockAndTakeSellersInfo(IClientSessionHandle session, bool isBuy, List<ProductSimple> productsSimple)
        {
            if (isBuy)
            {
                return await SellProducts(session, productsSimple);
            }
            else
            {
                return await RemoveProducts(session, productsSimple);
            }
        }

        private async Task<List<TransferInfo>> SellProducts(IClientSessionHandle session, List<ProductSimple> productsSimple)
        {
            var transferInfo = new List<TransferInfo>();
            foreach (var productSimple in productsSimple)
            {
                var product = await GetById(productSimple.Id);

                if (product.Stock < productSimple.Quantity)
                {
                    throw new Exception("This product is not available");
                }

                var transfer = new TransferInfo
                {
                    SellerId = product.SellerId,
                    Amount = productSimple.Price * productSimple.Quantity
                };
                transferInfo.Add(transfer);
                var productToUpdate = await _productRepository.UpdateStock(session, product.Id, -productSimple.Quantity);
                await _elasticSearch.Update(product.Id, productToUpdate);
            }

            return transferInfo;
        }

        private async Task<List<TransferInfo>> RemoveProducts(IClientSessionHandle session, List<ProductSimple> productsSimple)
        {
            var transferInfo = new List<TransferInfo>();
            foreach (var productSimple in productsSimple)
            {
                var product = await GetById(productSimple.Id);

                var transfer = new TransferInfo
                {
                    SellerId = product.SellerId,
                    Amount = productSimple.Price * productSimple.Quantity
                };

                transferInfo.Add(transfer);
                var productToUpdate = await _productRepository.UpdateStock(session, product.Id, productSimple.Quantity);
                await _elasticSearch.Update(product.Id, productToUpdate);
            }

            return transferInfo;
        }
    }
}
