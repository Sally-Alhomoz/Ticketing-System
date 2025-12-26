using Microsoft.Extensions.Logging;
using SharedDTOs;
using TicketingSystem.DataAccess.UnitOfWork;
using TicketingSystem.Services.Interfaces;
using TicketingSystem.DataAccess.Models;

namespace TicketingSystem.Services.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IUnitOfWork uow, ILogger<ProductService> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task AddProduct(ProductDto dto)
        {
            _logger.LogInformation("Adding a product.");

            var product = new Product
            {
                ProductName = dto.ProductName,
                IsDeleted=false
            };

            _uow.Products.Add(product);
            await _uow.Complete();
            _logger.LogInformation("Product added successfully.");
        }

        public async Task<bool> DeleteProduct(int productId)
        {
            _logger.LogInformation("Deleting a product.");

            var result = await _uow.Products.Delete(productId);

            if(!result)
            {
                _logger.LogWarning("Product not found.");
                return false;
            }

            _logger.LogInformation("Product deleted successfully.");
            await _uow.Complete();
            return true;
        }

        public async Task<(List<ProductDto> products, int totalCount)> GetProductPaged(
            int page = 1,
            int pageSize = 10,
            string search = "",
            string sortBy = "productName",
            string sortDirection = "asc")
        {
            _logger.LogInformation("Retrieving paged products. Page: {page}, Size: {pageSize}", page, pageSize);

            var query = _uow.Products.GetProducts();

            // GLOBAL SEARCH — all fields
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                query = query.Where(p =>
                    p.ProductName.ToLower().Contains(s)
                );
            }

            // FULL SORTING — every column
            query = (sortBy?.ToLower(), sortDirection?.ToLower()) switch
            {
                ("productName", "desc") => query.OrderByDescending(p => p.ProductName),
                ("productName", _) => query.OrderBy(p => p.ProductName)
            };

            int totalCount = query.Count();

            var products = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    ProductName = p.ProductName,
                    IsDeleted = p.IsDeleted
                }).ToList();

            return (products, totalCount);
        }

    }
}
