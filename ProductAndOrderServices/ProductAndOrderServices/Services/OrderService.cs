using AutoMapper;
using BankAccountService;
using FluentValidation;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using ProductAndOrderServices.Model;
using ProductAndOrderServices.Model.Dtos;
using ProductAndOrderServices.Repository.IRepository;
using ProductAndOrderServices.Services.IServices;
using System.Security.Claims;
using UserService;
using static BankAccountService.BankAccountService;
using static UserService.UserService;

namespace ProductAndOrderServices.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IValidator<OrderCreateDto> _createDtoValidator;
        private readonly IValidator<OrderUpdateDto> _updateDtoValidator;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IProductService _productService;
        private readonly BankAccountServiceClient _bankAccountClient;
        private readonly UserServiceClient _userClient;

        public OrderService(IOrderRepository orderRepository, IValidator<OrderCreateDto> createDtoValidator, IValidator<OrderUpdateDto> updateDtoValidator, IMapper mapper, IHttpContextAccessor httpContextAccessor, IProductService productService, BankAccountServiceClient bankAccountClient, UserServiceClient userClient)
        {
            _orderRepository = orderRepository;
            _createDtoValidator = createDtoValidator;
            _updateDtoValidator = updateDtoValidator;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _productService = productService;
            _bankAccountClient = bankAccountClient;
            _userClient = userClient;
        }

        public async Task<List<Order>> GetAll()
        {
            return await _orderRepository.GetAll();
        }

        public async Task<Order> GetById(string id)
        {
            return await _orderRepository.GetById(id);
        }

        #region POST
        public async Task Post(OrderCreateDto order)
        {
            await ValidateOrderAndUser(order);

            CheckProducts(order.ProductsCreateDto, out List<ProductSimple> products, out double totalPrice);

            var orderToAdd = _mapper.Map<Order>(order);

            orderToAdd.UserId = await GetUserId();
            orderToAdd.Products = products;
            orderToAdd.Price = totalPrice;

            await _orderRepository.Post(orderToAdd);
        }

        private async Task ValidateOrderAndUser(OrderCreateDto order)
        {
            var validator = await Task.Run(() => _createDtoValidator.Validate(order));

            if (!validator.IsValid)
            {
                throw new Exception(validator.ToString());
            }

            var userBankAccountId = await GetUsersBankAccountId();

            if (userBankAccountId.IsNullOrEmpty())
            {
                throw new Exception("You must first add a bank account");
            }
        }

        public async Task<string> GetUsersBankAccountId()
        {
            return await Task.Run(() =>
                   _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x =>
                   x.Type == "BankAccountId")?.Value
                   ?? "");
        }

        private void CheckProducts(List<ProductSimpleCreateDto> productsToCheck, out List<ProductSimple> productsToAdd, out double totalPrice)
        {
            totalPrice = 0;
            productsToAdd = new List<ProductSimple>();

            foreach (var product in productsToCheck)
            {
                var productSimple = _productService.GetProduct(product).Result;
                if (!productSimple.Id.IsNullOrEmpty())
                {
                    productsToAdd.Add(productSimple);
                    totalPrice += productSimple.Price * productSimple.Quantity;
                }
            }
            if (productsToAdd.Count <= 0)
            {
                throw new Exception("Products you selected are not available");
            }
        }
        #endregion

        public async Task Update(string id, OrderUpdateDto order)
        {
            ValidateOrderToUpdate(await GetById(id), order);

            var updateResult = await _orderRepository.Update(id, order);

            if (updateResult.ModifiedCount <= 0)
            {
                throw new Exception("Order not found");
            }
        }

        public void ValidateOrderToUpdate(Order order, OrderUpdateDto orderUpdateDto)
        {
            var validator = _updateDtoValidator.Validate(orderUpdateDto);

            if (!validator.IsValid)
            {
                throw new Exception(validator.ToString());
            }

            if (order.BoughtTime.AddMinutes(30) < DateTime.Now)
            {
                throw new Exception("You can not return this order, is being shipped");
            }
        }

        public async Task Delete(string id)
        {
            var deleteResult = await _orderRepository.Delete(id);

            if (deleteResult.DeletedCount <= 0)
            {
                throw new Exception("Order not found");
            }
        }

        public async Task BuyOrder(string id)
        {
            var userId = await GetUserId();
            var order = await GetById(id);

            if (!order.UserId.Equals(userId))
            {
                throw new Exception("This is not your order");
            }

            await Proced(id, true, order.Products);
        }

        public async Task ReturnOrder(string id)
        {
            var userId = await GetUserId();
            var order = await GetById(id);

            ValidateOrderAndUser(userId, order);

            await Proced(id, false, order.Products);
        }

        private void ValidateOrderAndUser(string userId, Order order)
        {
            if (!order.UserId.Equals(userId))
            {
                throw new Exception("This is not your order");
            }

            if (!order.IsBought)
            {
                throw new Exception("This order is not bought");
            }

            if (order.BoughtTime.AddMinutes(30) < DateTime.Now)
            {
                throw new Exception("You can not return this order, is being shipped");
            }
        }

        private async Task Proced(string id, bool isBuy, List<ProductSimple> products)
        {
            var buyerBankAccountId = await GetUsersBankAccountId() ?? "";

            var session = await _orderRepository.StartSession();
            
            try
            {
                session.StartTransaction();
                await UpdateOrder(session, isBuy, id);
                var transferInfo = await _productService.UpdateStockAndTakeSellersInfo(session, isBuy, products);
                await Transfer(buyerBankAccountId, isBuy, transferInfo);
                await session.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await session.AbortTransactionAsync();
                throw new Exception("Failed to bay/return the order\n" + ex.Message);
            }
        }

        private async Task UpdateOrder(IClientSessionHandle session, bool isBuy, string id)
        {
            if (isBuy)
            {
                var updateResult = await _orderRepository.BuyOrder(session, id);

                if (updateResult.ModifiedCount <= 0)
                {
                    throw new Exception("Order not found");
                }
            }
            else
            {
                var updateResult = await _orderRepository.ReturnOrder(session, id);

                if (updateResult.ModifiedCount <= 0)
                {
                    throw new Exception("Order not found");
                }
            }
        }

        private async Task Transfer(string buyerBankAccoundId, bool isBuy, List<TransferInfo> transferInfos)
        {
            var transferRequestList = new TransferRequestList();
            foreach (var transferInfo in transferInfos)
            {
                var sellerBankAccountId = await GetBankAccountId(transferInfo.SellerId);

                var request = new TransferRequest();

                if (isBuy)
                {
                    request.SenderBankAccountId = buyerBankAccoundId;
                    request.RecieverBankAccountId = sellerBankAccountId;
                }
                else
                {
                    request.SenderBankAccountId = sellerBankAccountId;
                    request.RecieverBankAccountId = buyerBankAccoundId;
                }
                request.Amount = transferInfo.Amount;
                transferRequestList.List.Add(request);
            }

            await _bankAccountClient.TransferAsync(transferRequestList);
        }

        private async Task<string> GetBankAccountId(string userId)
        {
            var request = new UserIdRequest();
            request.UserId = userId;
            var response = await _userClient.GetBankAccountIdAsync(request);
            return response.BankAccountId;
        }

        public async Task<List<Order>> GetMyOrders()
        {
            var userId = await GetUserId();
            return await _orderRepository.GetMyOrders(userId);
        }

        public async Task<Order> GetMyOrder(string id)
        {
            var userId = await GetUserId();
            var order = await GetById(id);

            if (!order.UserId.Equals(userId)) {
                throw new Exception("This not your order");
            }

            return order;
        }

        public async Task UpdateMyOrder(string id, OrderUpdateDto orderUpdateDto)
        {
            var userId = await GetUserId();
            var order = await GetById(id);

            if (!order.UserId.Equals(userId))
            {
                throw new Exception("This not your order");
            }

            ValidateOrderToUpdate(order, orderUpdateDto);

            await Update(id, orderUpdateDto);
        }

        public async Task RemoveProduct(string id, ProductSimpleCreateDto productSimple)
        {
            var userId = await GetUserId();
            var order = await GetById(id);

            ValidateOrderToUpdateProducts(userId, order);

            var product = GetProductToRemove(productSimple.Id, order.Products);

            var orderToUpdate = UpdateOrder(order, product, productSimple);

            var updateResult = await _orderRepository.RemoveProduct(id, orderToUpdate.Price, orderToUpdate.Products);

            if (updateResult.ModifiedCount <= 0)
            {
                throw new Exception("Order not found");
            }
        }

        private void ValidateOrderToUpdateProducts(string userId, Order order)
        {
            if (!order.UserId.Equals(userId))
            {
                throw new Exception("This not your order");
            }

            if (order.BoughtTime.AddMinutes(30) < DateTime.Now)
            {
                throw new Exception("You can not return this order, is being shipped");
            }
        }

        private ProductSimple GetProductToRemove(string id, List<ProductSimple> productSimples)
        {
            var products = productSimples.Where(p => p.Id.Equals(id));

            if (products == null)
            {
                throw new Exception("This product is not in this order");
            }

            if (products.ToList().Count > 1)
            {
                products = products.OrderByDescending(x => x.Price);
            }

            return products.ToList()[0];
        }

        private Order UpdateOrder(Order order, ProductSimple product, ProductSimpleCreateDto productToAdd)
        {
            var productQuantity = product.Quantity;
            product.Quantity -= productToAdd.Quantity;

            if (product.Quantity <= 0)
            {
                order.Products.Remove(product);
                order.Price -= product.Price * productQuantity;
            }
            else
            {
                order.Price -= product.Price * productToAdd.Quantity;
            }

            return order;
        }

        public async Task AddProduct(string id, ProductSimpleCreateDto productSimple)
        {
            var userId = await GetUserId();
            var order = await GetById(id);

            ValidateOrderToUpdateProducts(userId, order);

            var orderToUpdate = await GetProductToAdd(productSimple, order);

            var updateResult = await _orderRepository.AddProduct(id, orderToUpdate.Price, orderToUpdate.Products);

            if (updateResult.ModifiedCount <= 0)
            {
                throw new Exception("Order not found");
            }
        }

        private async Task<Order> GetProductToAdd(ProductSimpleCreateDto productSimple, Order order)
        {
            var productToAdd = await CheckProduct(productSimple);

            var product = order.Products.FirstOrDefault(p => p.Id.Equals(productToAdd.Id));

            if (product == null)
            {
                order.Products.Add(productToAdd);
                order.Price += productToAdd.Price * productToAdd.Quantity;
            }
            else
            {
                if (productToAdd.Price != product.Price)
                {
                    order.Products.Add(productToAdd);
                    order.Price += productToAdd.Price * productToAdd.Quantity;
                }
                else
                {
                    product.Quantity += productToAdd.Quantity;
                    order.Price += product.Price * productToAdd.Quantity;
                }
            }

            return order;
        }

        private async Task<ProductSimple> CheckProduct(ProductSimpleCreateDto productToCheck)
        {
            var productSimple = await _productService.GetProduct(productToCheck);

            if (productSimple.Id.IsNullOrEmpty())
            {
                throw new Exception("This product is not available");
            }

            return productSimple;
        }

        public async Task DeleteMyOrder(string id)
        {
            var userId = await GetUserId();
            var order = await GetById(id);

            if (!order.UserId.Equals(userId))
            {
                throw new Exception("This not your order");
            }

            await Delete(id);
        }

        private async Task<string> GetUserId()
        {
            return await Task.Run(() => _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x =>
                   x.Type == ClaimTypes.NameIdentifier)?.Value
                   ?? throw new Exception("You must login first"));
        }
    }
}
