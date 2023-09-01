using AutoMapper;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductAndOrderServices.Controllers;
using ProductAndOrderServices.Data;
using ProductAndOrderServices.ElasasticSearch;
using ProductAndOrderServices.Helpers;
using ProductAndOrderServices.Model;
using ProductAndOrderServices.Model.Dtos;
using ProductAndOrderServices.Repository;
using ProductAndOrderServices.Services;
using MongoDB.Driver;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TestProductAndOrderServices.Helpers;

namespace TestProductAndOrderServices
{
    public class TestProduct : IClassFixture<MongoDbFixture>,
        IClassFixture<ProductCreateDtoValidatorFixture>,
        IClassFixture<ProductUpdateDtoValidatorFixture>,
        IClassFixture<AutoMapperFixture>
    {
        private readonly ProductService _productService;
        private readonly ProductController _productController;

        private readonly MongoDbFixture _fixture;
        private readonly AutoMapperFixture _autoMapperFixture;
        private readonly ProductCreateDtoValidatorFixture _createDtoValidatorFixture;
        private readonly ProductUpdateDtoValidatorFixture _updateDtoValidatorFixture;

        private readonly HttpContextAccessorHelper _httpContextAccessorHelper;

        private readonly IValidator<ProductCreateDto> _validatorCreate;
        private readonly IValidator<ProductUpdateDto> _validatorUpdate;
        private readonly ECommerceDbContext _eCommerceDbContext;
        private readonly ProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ElasticSearch _elasticSearch;

        public TestProduct(ProductCreateDtoValidatorFixture createDtoValidatorFixture, ProductUpdateDtoValidatorFixture updateDtoValidatorFixture, AutoMapperFixture autoMapperFixture)
        {
            _createDtoValidatorFixture = createDtoValidatorFixture;
            _updateDtoValidatorFixture = updateDtoValidatorFixture;
            _autoMapperFixture = autoMapperFixture;

            _httpContextAccessorHelper = new HttpContextAccessorHelper();

            _fixture = new MongoDbFixture();
            var client = new MongoClient(_fixture.ConnectionString);
            _eCommerceDbContext = new ECommerceDbContext(client);
            _productRepository = new ProductRepository(_eCommerceDbContext);

            _elasticSearch = A.Fake<ElasticSearch>();

            _validatorCreate = _createDtoValidatorFixture.ServiceProvider.GetRequiredService<IValidator<ProductCreateDto>>();
            _validatorUpdate = _updateDtoValidatorFixture.ServiceProvider.GetRequiredService<IValidator<ProductUpdateDto>>();
            _mapper = _autoMapperFixture.ServiceProvider.GetRequiredService<IMapper>();
            _httpContextAccessor = _httpContextAccessorHelper.HttpContextAccessor;

            _productService = new ProductService(_productRepository,
                                    _validatorCreate,
                                    _validatorUpdate,
                                    _mapper,
                                    _httpContextAccessor,
                                    _elasticSearch);

            _productController = new ProductController(_productService);
        }

        [Fact]
        public async Task Test_GetAll()
        {
            var product1 = new Product
            {
                Id = "64de2e41582aeaab5e392985",
                Name = "Name",
                Stock = 1,
                BasePrice = 1,
                Discount = 0,
                Price = 1,
                Description = "Description",
                SellerId = "SellerId",
                Specificities = new Dictionary<string, string>
                {
                    {"Key", "Value" }
                }
            };

            var product2 = new Product
            {
                Id = "64de2e41582aeaab5e392986",
                Name = "Name",
                Stock = 1,
                BasePrice = 1,
                Discount = 0,
                Price = 1,
                Description = "Description",
                SellerId = "SellerId",
                Specificities = new Dictionary<string, string>
                {
                    {"Key", "Value" }
                }
            };

            await _productRepository.Post(product1);
            await _productRepository.Post(product2);

            SearchAndSort searchAndSort = new SearchAndSort();

            var result = await _productController.GetAll(searchAndSort);

            Assert.IsType<OkObjectResult>(result);

            var list = result as OkObjectResult;
            Assert.IsType<PagedInfo<Product>>(list.Value);

            var pagedInfo = await _productRepository.GetAll(searchAndSort);
            Assert.Equal(2, pagedInfo.TotalCount);
        }

        [Theory]
        [InlineData("64de2e41582aeaab5e392985", "64de2e41582aeaab5e392988")]
        public async Task Test_GetById(string okId, string notOkId)
        {
            var product = new Product
            {
                Id = okId,
                Name = "Name",
                Stock = 1,
                BasePrice = 1,
                Discount = 0,
                Price = 1,
                Description = "Description",
                SellerId = "SellerId",
                Specificities = new Dictionary<string, string>
                {
                    {"Key", "Value" }
                }
                
            };

            await _productRepository.Post(product);

            var result = await _productController.GetById(okId);
            Assert.IsType<OkObjectResult>(result);

            var errorResult = await _productController.GetById(notOkId);
            Assert.IsType<BadRequestObjectResult>(errorResult);
        }

        [Theory]
        [InlineData("Product", 3, 4.5)]
        public async Task Test_PostTrue(string name, int stock, double basePrice)
        {
            var productCreateDto = new ProductCreateDto
            {
                Name = name,
                Stock = stock,
                BasePrice = basePrice
            };

            var result = await _productController.Post(productCreateDto);
            Assert.IsType<OkObjectResult>(result);
        }

        [Theory]
        [InlineData(null, 4, 1.1)]
        [InlineData("Product", 0, 5.0)]
        [InlineData("Product", 10, -5.3)]
        [InlineData("", 2, 12.1)]
        public async Task Test_PostFalse(string name, int stock, double basePrice)
        {
            var productCreateDto = new ProductCreateDto
            {
                Name = name,
                Stock = stock,
                BasePrice = basePrice
            };

            var result = await _productController.Post(productCreateDto);
            Assert.IsType<BadRequestObjectResult>(result);
        }


        [Theory]
        [InlineData("Product", "Description", 10)]
        [InlineData(null, "Description", 10)]
        [InlineData("Product", null, 10)]
        public async Task Test_UpdateTrue(string name, string description, int discount)
        {
            var product = new Product
            {
                Id = "64ef01d2b047c5341c0913e9",
                Name = "Name",
                Stock = 1,
                BasePrice = 1,
                Discount = 0,
                Price = 1,
                Description = "Description",
                SellerId = "SellerId",
                Specificities = new Dictionary<string, string>
                {
                    {"Key", "Value" }
                }

            };

            await _productRepository.Post(product);

            var productToUpdate = new ProductUpdateDto
            {
                Name = name,
                Description = description,
                Discount = discount
            };

            var result = await _productController.Update(product.Id, productToUpdate);
            Assert.IsType<OkObjectResult>(result);
        }

        [Theory]
        [InlineData("Product", "Description", 120)]
        [InlineData("Product", "Description", -9)]
        [InlineData(null, null, null)]
        public async Task Test_UpdateFalse(string name, string description, int? discount)
        {
            var product = new Product
            {
                Id = "64ef01d2b047c5341c0913e9",
                Name = "Name",
                Stock = 1,
                BasePrice = 1,
                Discount = 0,
                Price = 1,
                Description = "Description",
                SellerId = "SellerId",
                Specificities = new Dictionary<string, string>
                {
                    {"Key", "Value" }
                }

            };

            await _productRepository.Post(product);

            var productToUpdate = new ProductUpdateDto
            {
                Name = name,
                Description = description,
                Discount = discount
            };

            var result = await _productController.Update(product.Id, productToUpdate);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Theory]
        [InlineData("64ef01d2b047c5341c0913e6", "64ef01d2b047c5341c0913e7")]
        public async Task Test_Delete(string okId, string notOkId)
        {
            var product1 = new Product
            {
                Id = okId,
                Name = "Name",
                Stock = 1,
                BasePrice = 1,
                Discount = 0,
                Price = 1,
                Description = "Description",
                SellerId = "SellerId",
                Specificities = new Dictionary<string, string>
                {
                    {"Key", "Value" }
                }
            };

            var product2 = new Product
            {
                Id = "64ef01d2b047c5341c0913e8",
                Name = "Name",
                Stock = 1,
                BasePrice = 1,
                Discount = 0,
                Price = 1,
                Description = "Description",
                SellerId = "SellerId",
                Specificities = new Dictionary<string, string>
                {
                    {"Key", "Value" }
                }
            };

            await _productRepository.Post(product1);
            await _productRepository.Post(product2);

            var result = await _productController.Delete(okId);
            Assert.IsType<OkObjectResult>(result);

            var notOkResult = await _productController.Delete(notOkId);
            Assert.IsType<BadRequestObjectResult>(notOkResult);

            var pagedInfo = await _productRepository.GetAll(new SearchAndSort());

            Assert.Equal(1, pagedInfo.TotalCount);
        }
    }
}
