using UserAndBankAccountServices.Model.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAndBankAccountServices.Services.IServices;

namespace UserAndBankAccountServices.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    [Authorize]
    public class BankAccountController : Controller
    {
        private readonly IBankAccountService _bankAccountService;

        public BankAccountController(IBankAccountService bankAccountService)
        {
            _bankAccountService = bankAccountService;
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var bankAccounts = await _bankAccountService.GetAll();
                return Ok(bankAccounts);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var bankAccount = await _bankAccountService.GetById(id);
                return Ok(bankAccount);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("my-account")]
        public async Task<IActionResult> GetMyBankAccount()
        {
            try
            {
                var bankAccount = await _bankAccountService.GetMyBankAccount();
                return Ok(bankAccount);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } 

        [HttpPost]
        public async Task<IActionResult> Post(BankAccountDto bankAccount)
        {
            try
            {
                await _bankAccountService.Post(bankAccount);
                return Ok("Posted sucefully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<IActionResult> Update(string id, BankAccountDto bankAccount)
        {
            try
            {
                await _bankAccountService.Update(id, bankAccount);
                return Ok("Updated successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _bankAccountService.Delete(id);
                return Ok("Deleted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}/deposite")]
        public async Task<IActionResult> Deposite(double amount)
        {
            try
            {
                await _bankAccountService.Deposite(amount);
                return Ok("Deposited successfully");
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}/withdraw")]
        public async Task<IActionResult> Withdraw(double amount)
        {
            try
            {
                await _bankAccountService.Withdraw(amount);
                return Ok("Withdrawed successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
