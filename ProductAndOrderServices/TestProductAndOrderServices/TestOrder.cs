using AutoMapper;
using FakeItEasy;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using ProductAndOrderServices.Controllers;
using ProductAndOrderServices.Data;
using ProductAndOrderServices.ElasasticSearch;
using ProductAndOrderServices.Model;
using ProductAndOrderServices.Model.Dtos;
using ProductAndOrderServices.Repository;
using ProductAndOrderServices.Services;
using TestProductAndOrderServices.Helpers;
using static BankAccountService.BankAccountService;
using static UserService.UserService;

namespace TestProductAndOrderServices
{
    public class TestOrder : IClassFixture<MongoDbFixture>,
                             IClassFixture<OrderCreateDtoValidatorFixture>,
                             IClassFixture<OrderUpdateDtoValidatorFixture>,
                             IClassFixture<AutoMapperFixture>
    {
        private readonly OrderService _orderService;
        private readonly OrderController _orderController;

        private readonly MongoDbFixture _fixture;
        private readonly OrderCreateDtoValidatorFixture _orderCreateDtoValidatorFixture;
        private readonly OrderUpdateDtoValidatorFixture _orderUpdateDtoValidatorFixture;
        private readonly AutoMapperFixture _autoMapperFixture;

        private readonly HttpContextAccessorHelper _httpContextAccessorHelper;

        private readonly ECommerceDbContext _eCommerceDbContext;
        private readonly OrderRepository _orderRepository;
        private readonly ProductRepository _productRepository;

        private readonly IValidator<OrderCreateDto> _createDtoValidator;
        private readonly IValidator<OrderUpdateDto> _updateDtoValidator;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ProductService _productService;
        private readonly BankAccountServiceClient _bankAccountClient;
        private readonly UserServiceClient _userClient;

        private readonly IValidator<ProductCreateDto> _validatorCreate;
        private readonly IValidator<ProductUpdateDto> _validatorUpdate;
        private readonly ElasticSearch _elasticSearch;

        public TestOrder(OrderCreateDtoValidatorFixture orderCreateDtoValidatorFixture, OrderUpdateDtoValidatorFixture orderUpdateDtoValidatorFixture, AutoMapperFixture autoMapperFixture)
        {
            _fixture = new MongoDbFixture();
            var client = new MongoClient(_fixture.ConnectionString);
            _eCommerceDbContext = new ECommerceDbContext(client);
            _orderRepository = new OrderRepository(_eCommerceDbContext);
            _productRepository = new ProductRepository(_eCommerceDbContext);

            _orderCreateDtoValidatorFixture = orderCreateDtoValidatorFixture;
            _orderUpdateDtoValidatorFixture = orderUpdateDtoValidatorFixture;
            _autoMapperFixture = autoMapperFixture;

            _createDtoValidator = _orderCreateDtoValidatorFixture.ServiceProvider.GetRequiredService<IValidator<OrderCreateDto>>();
            _updateDtoValidator = _orderUpdateDtoValidatorFixture.ServiceProvider.GetRequiredService<IValidator<OrderUpdateDto>>();
            _mapper = _autoMapperFixture.ServiceProvider.GetRequiredService<IMapper>();

            _httpContextAccessorHelper = new HttpContextAccessorHelper();
            _httpContextAccessor = _httpContextAccessorHelper.HttpContextAccessor;

            _bankAccountClient = A.Fake<BankAccountServiceClient>();
            _userClient = A.Fake<UserServiceClient>();

            _validatorCreate = A.Fake<IValidator<ProductCreateDto>>();
            _validatorUpdate = A.Fake<IValidator<ProductUpdateDto>>();
            _elasticSearch = A.Fake<ElasticSearch>();

            _productService = new ProductService(_productRepository,
                            _validatorCreate,
                            _validatorUpdate,
                            _mapper,
                            _httpContextAccessor,
                            _elasticSearch);

            _orderService = new OrderService(_orderRepository,
                _createDtoValidator,
                _updateDtoValidator,
                _mapper,
                _httpContextAccessor,
                _productService,
                _bankAccountClient,
                _userClient);

            _orderController = new OrderController(_orderService);
        }

        [Fact]
        public async Task Test_GetAll()
        {
            var order1 = new Order
            {
                Id = "64e09804777443066837fd53",
                Address = "Address",
                Price = 1,
                UserId = "UserId",
                IsBought = false,
                BoughtTime = DateTime.Now,
                Products = null
            };
            var order2 = new Order
            {
                Id = "64e09804777443066837fd54",
                Address = "Address",
                Price = 1,
                UserId = "UserId",
                IsBought = false,
                BoughtTime = DateTime.Now,
                Products = null
            };

            await _orderRepository.Post(order1);
            await _orderRepository.Post(order2);

            var result = await _orderController.GetAll();
            Assert.IsType<OkObjectResult>(result);

            var orders = await _orderRepository.GetAll();
            Assert.Equal(2, orders.Count);
        }

        [Theory]
        [InlineData("Address", 3)]
        public async Task Test_PostOk(string address, int quantity)
        {
            var product = new Product
            {
                Id = "64e09804777443066837fd59",
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

            var productsSimpleDto = new List<ProductSimpleCreateDto>()
            {
                new ProductSimpleCreateDto
                {
                    Id = "64e09804777443066837fd59",
                    Quantity = quantity
                }
            };

            var orderCreateDto = new OrderCreateDto
            {
                Address = address,
                ProductsCreateDto = productsSimpleDto
            };

            var result = await _orderController.Post(orderCreateDto);
            Assert.IsType<OkObjectResult>(result);
        }

        [Theory]
        [InlineData(null, 5)]
        [InlineData("Address", 0)]
        [InlineData("Address", 101)]
        public async Task Test_PostNotOk(string address, int quantity)
        {
            var productsSimpleDto = new List<ProductSimpleCreateDto>()
            {
                new ProductSimpleCreateDto
                {
                    Id = "",
                    Quantity = quantity
                }
            };

            var orderCreateDto = new OrderCreateDto
            {
                Address = address,
                ProductsCreateDto = productsSimpleDto
            };

            var result = await _orderController.Post(orderCreateDto);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Theory]
        [InlineData("NewAddress")]
        public async Task Test_UpdateOk(string address)
        {
            var order = new Order
            {
                Id = "64e09804777443066837fd54",
                Address = "Address",
                Price = 1,
                UserId = "UserId",
                IsBought = false,
                BoughtTime = DateTime.Now.AddDays(1),
                Products = null
            };

            await _orderRepository.Post(order);

            var orderUpdateDto = new OrderUpdateDto
            {
                Address = address
            };

            var result = await _orderController.Update(order.Id, orderUpdateDto);
            Assert.IsType<OkObjectResult>(result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("NewAddress")]
        public async Task Test_UpdateNotOk(string address)
        {
            var order = new Order
            {
                Id = "64e09804777443066837fd54",
                Address = "Address",
                Price = 1,
                UserId = "UserId",
                IsBought = false,
                BoughtTime = DateTime.Now.AddMinutes(-31),
                Products = null
            };

            await _orderRepository.Post(order);

            var orderUpdateDto = new OrderUpdateDto
            {
                Address = address
            };

            var result = await _orderController.Update(order.Id, orderUpdateDto);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Theory]
        [InlineData("64dcd34fe55c1e2ee8460998", "64dcd34fe55c1e2ee8460999")]
        public async Task Test_GetById(string okId, string notOkId)
        {
            var order = new Order
            {
                Id = okId,
                Address = "Address",
                Price = 1,
                UserId = "UserId",
                IsBought = false,
                BoughtTime = DateTime.Now,
                Products = null
            };

            await _orderRepository.Post(order);

            var result = await _orderController.GetById(okId);
            var notOkResult = await _orderController.GetById(notOkId);
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<BadRequestObjectResult>(notOkResult);
        }

        [Theory]
        [InlineData("64dcd34fe55c1e2ee8460997", "64dcd34fe55c1e2ee8460999")]
        public async Task Test_Delete(string okId, string notOkId)
        {
            var order1 = new Order
            {
                Id = okId,
                Address = "Address",
                Price = 1,
                UserId = "UserId",
                IsBought = false,
                BoughtTime = DateTime.Now,
                Products = null
            };
            var order2 = new Order
            {
                Id = "64dcd34fe55c1e2ee8460994",
                Address = "Address",
                Price = 1,
                UserId = "UserId",
                IsBought = false,
                BoughtTime = DateTime.Now,
                Products = null
            };

            await _orderRepository.Post(order1);
            await _orderRepository.Post(order2);

            var result = await _orderController.Delete(okId);
            var notOkResult = await _orderController.Delete(notOkId);
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<BadRequestObjectResult>(notOkResult);

            var orders = await _orderRepository.GetAll();
            Assert.Single(orders);
        }
    }
}
