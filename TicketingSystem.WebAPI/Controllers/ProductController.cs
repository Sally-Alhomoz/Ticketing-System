using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SharedDTOs;
using TicketingSystem.DataAccess.Models;
using TicketingSystem.Services.Interfaces;

namespace TicketingSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productManager;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productManager, ILogger<ProductController> logger)
        {
            _productManager = productManager;
            _logger = logger;
        }

        /// <summary>
        /// Add product.
        /// </summary>
        [Authorize]
        [HttpPost("Add")]
        public async Task<IActionResult> Create(string name)
        {
            _logger.LogInformation("Adding product.");

            try
            {
                await _productManager.AddProduct(name);
                _logger.LogInformation("Product added successfully.");
                return Ok("Product added successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding a product.");
                return StatusCode(500, "An internal error occurred while creating the product.");
            }
        }

        /// <summary>
        /// Deleting a product.
        /// </summary>
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            _logger.LogInformation("Deleting product.");

            var result = await _productManager.DeleteProduct(id);

            if(!result)
            {
                _logger.LogWarning("Product not found.");
                return NotFound();
            }

            _logger.LogInformation("Product Deleted successfully.");
            return Ok("Product Deleted successfully.");
        }

        /// <summary>
        /// Get products.
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Read(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string search = "",
            [FromQuery] string sortBy = "productName",
            [FromQuery] string sortDirection = "asc")
        {
            var (products, totalCount) = await _productManager.GetProductPaged(page, pageSize, search, sortBy, sortDirection);

            return Ok(new
            {
                items = products,
                totalCount = totalCount
            });
        }
    }
}
