using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SharedDTOs;
using Swashbuckle.AspNetCore.Annotations;
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

        [Authorize]
        [HttpPost("Add")]
        [SwaggerOperation(
            Summary = "Add a product.",
            Description = "Adds a new product to the system. Authentication required.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Product added successfully")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error occurred")]
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


        [Authorize]
        [HttpDelete]
        [SwaggerOperation(
            Summary = "Delete a product.",
            Description = "Delete a product from the system.. Authentication required.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Product delted successfully")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Product not found")]
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

        [Authorize]
        [SwaggerOperation(
            Summary = "List all products.",
            Description = "Returns a paginated list of all products. restricted to administrative roles.")]
        [SwaggerResponse(StatusCodes.Status200OK, "List of products retrieved successfully")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required")]
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
