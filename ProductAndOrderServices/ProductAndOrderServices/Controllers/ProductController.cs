using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductAndOrderServices.Helpers;
using ProductAndOrderServices.Model.Dtos;
using ProductAndOrderServices.Services.IServices;

namespace ProductAndOrderServices.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] SearchAndSort searchAndSort)
        {
            try
            {
                var products = await _productService.GetAll(searchAndSort);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var product = await _productService.GetById(id);
                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(ProductCreateDto product)
        {
            try
            {
                await _productService.Post(product);
                return Ok("Posted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> Update(string id, [FromForm] ProductUpdateDto product)
        {
            try
            {
                await _productService.Update(id, product);
                return Ok("Updated successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _productService.Delete(id);
                return Ok("Deleted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("my-products")]
        public async Task<IActionResult> GetMyProducts()
        {
            try
            {
                var products = await _productService.GetMyProducts();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("my-products/{id}")]
        public async Task<IActionResult> UpdateMyProduct(string id, [FromForm] ProductUpdateDto product)
        {
            try
            {
                await _productService.UpdateMyProduct(id, product);
                return Ok("Updated successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("my-products/{id}")]
        public async Task<IActionResult> DeleteMyProduct(string id)
        {
            try
            {
                await _productService.DeleteMyProduct(id);
                return Ok("Deleted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
