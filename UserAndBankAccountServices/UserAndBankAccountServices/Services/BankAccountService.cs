using AutoMapper;
using UserAndBankAccountServices.Models;
using FluentValidation;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MongoDB.Bson;
using System.Security.Claims;
using UserAndBankAccountServices.Model.Dtos;
using static BankAccountService.BankAccountService;
using BankAccountService;
using UserAndBankAccountServices.Services.IServices;
using UserAndBankAccountServices.Repository.IRepository;
using Azure.Core;

namespace UserAndBankAccountServices.Service
{
    public class BankAccountService : BankAccountServiceBase, IBankAccountService
    {
        private readonly IBankAccountRepository _bankAccountRepository;
        private readonly IValidator<BankAccountDto> _validator;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;

        public BankAccountService(IBankAccountRepository bankAccountRepository, IValidator<BankAccountDto> validator, IMapper mapper, IHttpContextAccessor httpContextAccessor, IUserService userService)
        {
            _bankAccountRepository = bankAccountRepository;
            _validator = validator;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
        }

        public async Task<List<BankAccount>> GetAll()
        {
            return await _bankAccountRepository.GetAll();
        }

        public async Task<BankAccount> GetById(string id)
        {
            return await _bankAccountRepository.GetById(id);
        }

        public async Task<BankAccount> GetMyBankAccount()
        {
            var id = GetId();

            return await GetById(id);
        }

        private string GetId()
        {
            return _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x =>
                   x.Type == "BankAccountId")?.Value ?? "";
        }

        public async Task Post(BankAccountDto bankAccount)
        {
            var validator = _validator.Validate(bankAccount);

            if (!validator.IsValid)
            {
                throw new Exception(validator.ToString());
            }

            var bankAccountToAdd = _mapper.Map<BankAccount>(bankAccount);
            bankAccountToAdd.Id = ObjectId.GenerateNewId().ToString();

            var userId = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x =>
                         x.Type == ClaimTypes.NameIdentifier)?.Value
                         ?? throw new Exception("You must login again");

            var transaction = await _bankAccountRepository.BeginTransaction();
            try
            {
                await _bankAccountRepository.Post(bankAccountToAdd);
                await _userService.AddBankAccountId(userId, bankAccountToAdd.Id);
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }

        public async Task Update(string id, BankAccountDto bankAccount)
        {
            var validator = _validator.Validate(bankAccount);

            if (!validator.IsValid)
            {
                throw new Exception(validator.ToString());
            }

            await _bankAccountRepository.Update(id, bankAccount);
        }

        public async Task Delete(string id)
        {
            await _bankAccountRepository.Delete(id);
        }

        public async Task Deposite(double amount)
        {
            if (amount <= 0)
            {
                throw new Exception("You can not deposite negative/zero value");
            }

            var id = GetId();
            var bankAccountDto = _mapper.Map<BankAccountDto>(await GetById(id));
            bankAccountDto.Amount += amount;

            await _bankAccountRepository.Deposite(id, bankAccountDto);
        }

        public async Task Withdraw(double amount)
        {
            if (amount <= 0)
            {
                throw new Exception("You can not withdraw negative/zero value");
            }

            var id = GetId();
            var bankAccountDto = _mapper.Map<BankAccountDto>(await GetById(id));
            var amountLeft = bankAccountDto.Amount - amount;

            if (amountLeft < 0)
            {
                throw new Exception("You have not enough founds, you have only: " + bankAccountDto.Amount + "$");
            }

            bankAccountDto.Amount = amountLeft;

            await _bankAccountRepository.Withdraw(id, bankAccountDto);
        }

        public override async Task<Empty> Transfer(TransferRequestList requestList, ServerCallContext context)
        {
            var transaction = await _bankAccountRepository.BeginTransaction();

            try
            {
                foreach (var request in requestList.List)
                {
                    await Withdraw(request.SenderBankAccountId, request.Amount);
                    await Deposite(request.RecieverBankAccountId, request.Amount);
                }
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw new Exception("Transaction failled");
            }


            return await Task.FromResult(new Empty());
        }

        public async Task Deposite(string id, double amount)
        {
            var bankAccountDto = _mapper.Map<BankAccountDto>(await GetById(id));
            bankAccountDto.Amount += amount;

            await _bankAccountRepository.Deposite(id, bankAccountDto);
        }

        public async Task Withdraw(string id, double amount)
        {
            var bankAccountDto = _mapper.Map<BankAccountDto>(await GetById(id));
            var amountLeft = bankAccountDto.Amount - amount;

            if (amountLeft < 0)
            {
                throw new Exception("You have not enough founds, you have only: " + bankAccountDto.Amount + "$");
            }

            bankAccountDto.Amount = amountLeft;

            await _bankAccountRepository.Withdraw(id, bankAccountDto);
        }
    }
}
