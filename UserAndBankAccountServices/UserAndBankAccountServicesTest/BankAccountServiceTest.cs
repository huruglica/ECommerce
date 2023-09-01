using AutoMapper;
using FakeItEasy;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography;
using UserAndBankAccountServices.Controllers;
using UserAndBankAccountServices.Data;
using UserAndBankAccountServices.Model.Dtos;
using UserAndBankAccountServices.Models;
using UserAndBankAccountServices.Repository;
using UserAndBankAccountServices.Services.IServices;
using UserAndBankAccountServicesTest.Helpers;

namespace UserAndBankAccountServicesTest
{
    public class BankAccountServiceTest : IClassFixture<BankAccountDtoValidatorFixture>,
                                          IClassFixture<AutoMapperFixture>
    {
        private readonly UserAndBankAccountServices.Service.BankAccountService _bankAccountService;
        private readonly BankAccountController _bankAccountController;

        private readonly ECommerceDbContex _eCommerceDbContex;

        private readonly BankAccountDtoValidatorFixture _validatorFixture;
        private readonly AutoMapperFixture _autoMapperFixture;

        private readonly HttpContextAccessorHelper _httpContextAccessorHelper;

        private readonly BankAccountRepository _bankAccountRepository;
        private readonly IValidator<BankAccountDto> _validator;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;

        public BankAccountServiceTest(BankAccountDtoValidatorFixture validatorFixture, AutoMapperFixture autoMapperFixture)
        {
            DbContextOptionsBuilder<ECommerceDbContex> dbOptions = new DbContextOptionsBuilder<ECommerceDbContex>()
                        .UseInMemoryDatabase(Guid.NewGuid().ToString());

            _eCommerceDbContex = new ECommerceDbContex(dbOptions.Options);
            _bankAccountRepository = new BankAccountRepository(_eCommerceDbContex);

            _validatorFixture = validatorFixture;
            _autoMapperFixture = autoMapperFixture;

            _httpContextAccessorHelper = new HttpContextAccessorHelper();

            _validator = _validatorFixture.ServiceProvider.GetRequiredService<IValidator<BankAccountDto>>();
            _mapper = _autoMapperFixture.ServiceProvider.GetRequiredService<IMapper>();

            _httpContextAccessor = _httpContextAccessorHelper.HttpContextAccessor;

            _userService = A.Fake<IUserService>();

            _bankAccountService = new UserAndBankAccountServices.Service.BankAccountService(_bankAccountRepository,
                                    _validator,
                                    _mapper,
                                    _httpContextAccessor,
                                    _userService);
            _bankAccountController = new BankAccountController(_bankAccountService);
        }

        [Fact]
        public async Task Test_GetAll()
        {
            var bankAccountOne = new BankAccount
            {
                Id = Guid.NewGuid().ToString(),
                Amount = 1
            };

            var bankAccountTwo = new BankAccount
            {
                Id = Guid.NewGuid().ToString(),
                Amount = 2
            };

            await _bankAccountRepository.Post(bankAccountOne);
            await _bankAccountRepository.Post(bankAccountTwo);

            var result = await _bankAccountController.GetAll();
            Assert.IsType<OkObjectResult>(result);

            var bankAccounts = await _bankAccountRepository.GetAll();
            Assert.Equal(2, bankAccounts.Count);
        }

        [Theory]
        [InlineData("64f1bf452c45efd1d18f86c1", "64f1bf452c45efd1d18f86c2")]
        public async Task Test_GetById(string okId, string notOkId)
        {
            var bankAccount = new BankAccount
            {
                Id = okId,
                Amount = 1
            };

            await _bankAccountRepository.Post(bankAccount);

            var result = await _bankAccountController.GetById(okId);
            var notOkResult = await _bankAccountController.GetById(notOkId);

            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<BadRequestObjectResult>(notOkResult);
        }

        [Fact]
        public async Task Test_GetMyBankAccountOk()
        {
            var bankAccount = new BankAccount
            {
                Id = "64dcd34fe55c1e2ee8460999",
                Amount = 1
            };

            await _bankAccountRepository.Post(bankAccount);

            var result = await _bankAccountController.GetMyBankAccount();
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Test_GetMyBankAccountNotOk()
        {
            var bankAccount = new BankAccount
            {
                Id = "64dcd34fe55c1e2ee8460990",
                Amount = 1
            };

            await _bankAccountRepository.Post(bankAccount);

            var result = await _bankAccountController.GetMyBankAccount();
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        [InlineData(100)]
        public async Task Test_PostOk(double amount)
        {
            var bankAccountDto = new BankAccountDto
            {
                Amount = amount
            };

            var result = await _validator.ValidateAsync(bankAccountDto);
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData(-100)]
        public async Task Test_PostNotOk(double amount)
        {
            var bankAccountDto = new BankAccountDto
            {
                Amount = amount
            };

            var result = await _bankAccountController.Post(bankAccountDto);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        [InlineData(100)]
        public async Task Test_UpdateOk(double amount)
        {
            var bankAccount = new BankAccount
            {
                Id = Guid.NewGuid().ToString(),
                Amount = 100
            };

            await _bankAccountRepository.Post(bankAccount);

            var bankAccountDto = new BankAccountDto
            {
                Amount = amount
            };

            var result = await _bankAccountController.Update(bankAccount.Id, bankAccountDto);
            Assert.IsType<OkObjectResult>(result);
        }

        [Theory]
        [InlineData(-100)]
        public async Task Test_UpdateNotOk(double amount)
        {
            var bankAccount = new BankAccount
            {
                Id = Guid.NewGuid().ToString(),
                Amount = 100
            };

            await _bankAccountRepository.Post(bankAccount);

            var bankAccountDto = new BankAccountDto
            {
                Amount = amount
            };

            var result = await _bankAccountController.Update(bankAccount.Id, bankAccountDto);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Theory]
        [InlineData("64f1bf452c45efd1d18f86c1", "64f1bf452c45efd1d18f86c2")]
        public async Task Test_Delete(string okId, string notOkId)
        {
            var bankAccount = new BankAccount
            {
                Id = okId,
                Amount = 1
            };

            await _bankAccountRepository.Post(bankAccount);

            var result = await _bankAccountController.Delete(okId);
            var notOkResult = await _bankAccountController.Delete(notOkId);

            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<BadRequestObjectResult>(notOkResult);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        public async Task Test_DepositeOk(double amount)
        {
            var bankAccount = new BankAccount
            {
                Id = "64dcd34fe55c1e2ee8460999",
                Amount = 1
            };

            await _bankAccountRepository.Post(bankAccount);

            var result = await _bankAccountController.Deposite(amount);
            Assert.IsType<OkObjectResult>(result);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public async Task Test_DepositeNotOk(double amount)
        {
            var bankAccount = new BankAccount
            {
                Id = "64dcd34fe55c1e2ee8460999",
                Amount = 1
            };

            await _bankAccountRepository.Post(bankAccount);

            var result = await _bankAccountController.Deposite(amount);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        public async Task Test_WithdrawOk(double amount)
        {
            var bankAccount = new BankAccount
            {
                Id = "64dcd34fe55c1e2ee8460999",
                Amount = 1000
            };

            await _bankAccountRepository.Post(bankAccount);

            var result = await _bankAccountController.Withdraw(amount);
            Assert.IsType<OkObjectResult>(result);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1000)]
        public async Task Test_WithdrawNotOk(double amount)
        {
            var bankAccount = new BankAccount
            {
                Id = "64dcd34fe55c1e2ee8460999",
                Amount = 100
            };

            await _bankAccountRepository.Post(bankAccount);

            var result = await _bankAccountController.Withdraw(amount);
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
