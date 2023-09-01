using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAndBankAccountServices.Models;
using UserAndBankAccountServices.Models.Dtos;
using UserAndBankAccountServices.Services.IServices;

namespace UserAndBankAccountServices.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var users = await _userService.GetAll();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var user = await _userService.GetById(id);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Post(UserCreateDto user)
        {
            try
            {
                await _userService.Post(user);
                return Ok("Posted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> SetRole(string id, string role)
        {
            try
            {
                await _userService.SetRole(id, role);
                return Ok("Updated successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _userService.Delete(id);
                return Ok("Deleted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserLogin userLogin)
        {
            try
            {
                return Ok(await _userService.Login(userLogin));
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("my-data")]
        public async Task<IActionResult> GetMyData()
        {
            try
            {
                var user = await _userService.GetMyData();
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update(UserUpdateDto user)
        {
            try
            {
                await _userService.Update(user);
                return Ok("Updated successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword(string newPassword)
        {
            try
            {
                await _userService.ChangePassword(newPassword);
                return Ok("Password changed");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete()
        {
            try
            {
                await _userService.Delete();
                return Ok("Deleted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
