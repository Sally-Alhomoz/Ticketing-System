using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SharedDTOs;
using TicketingSystem.Services.Interfaces;

namespace TicketingSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productManager;
        private readonly IConfiguration configuration;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productManager, IConfiguration config, ILogger<ProductController> logger)
        {
            _productManager = productManager;
            configuration = config;
            _logger = logger;
        }

        /// <summary>
        /// Add product.
        /// </summary>
        [Authorize]
        [HttpPost("Add")]
        public async Task<IActionResult> AddProduct(ProductDto product)
        {
            _logger.LogInformation("Adding product.");

            await _productManager.AddProduct(product);
            _logger.LogInformation("Product added successfully.");
            return Ok("Product added successfully.");
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
