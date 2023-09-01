using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductAndOrderServices.Model.Dtos;
using ProductAndOrderServices.Services.IServices;

namespace ProductAndOrderServices.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var orders = await _orderService.GetAll();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var order = await _orderService.GetById(id);
                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(OrderCreateDto order)
        {
            try
            {
                await _orderService.Post(order);
                return Ok("Posted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> Update(string id, OrderUpdateDto order)
        {
            try
            {
                await _orderService.Update(id, order);
                return Ok("Updated successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _orderService.Delete(id);
                return Ok("Deleted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            try
            {
                var orders = await _orderService.GetMyOrders();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("my-orders/{id}")]
        public async Task<IActionResult> GetMyOrder(string id)
        {
            try
            {
                var order = await _orderService.GetMyOrder(id);
                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("my-orders/{id}")]
        public async Task<IActionResult> UpdateMyOrder(string id, OrderUpdateDto order)
        {
            try
            {
                await _orderService.UpdateMyOrder(id, order);
                return Ok("Updated successfully");
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}/remove-product/")]
        public async Task<IActionResult> RemoveProduct(string id, ProductSimpleCreateDto product)
        {
            try
            {
                await _orderService.RemoveProduct(id, product);
                return Ok("Removed successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}/add-product/")]
        public async Task<IActionResult> AddProduct(string id, ProductSimpleCreateDto product)
        {
            try
            {
                await _orderService.AddProduct(id, product);
                return Ok("Added successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("my-orders/{id}")]
        public async Task<IActionResult> DeleteMyOrder(string id)
        {
            try
            {
                await _orderService.DeleteMyOrder(id);
                return Ok("Deleted successfully");
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}/buy-order")]
        public async Task<IActionResult> BuyOrder(string id)
        {
            try
            {
                await _orderService.BuyOrder(id);
                return Ok("Order is bought successfully");
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}/return-order")]
        public async Task<IActionResult> ReturnOrder(string id)
        {
            try
            {
                await _orderService.ReturnOrder(id);
                return Ok("Order is returned successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
