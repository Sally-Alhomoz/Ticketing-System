using Microsoft.Extensions.Logging;
using TicketingSystem.DataAccess.Interfaces;
using TicketingSystem.DataAccess.Models;


namespace TicketingSystem.DataAccess.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly TicketingSystemDBContext _db;
        private readonly ILogger<ProductRepository> _logger;

        public ProductRepository(TicketingSystemDBContext db, ILogger<ProductRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public void Add(Product product)
        {
            _logger.LogInformation("Adding a product.");
            _db.Products.Add(product);
            _logger.LogInformation("Product added successfully");
        }

        public bool Delete(int id)
        {
            _logger.LogInformation("Deleting a product from the database.");

            var product = _db.Products.FirstOrDefault(x => x.Id == id);

            if (product == null)
            {
                _logger.LogWarning("Product not found.");
                return false;
            }

            product.IsDeleted = true;
            _logger.LogInformation("Product with id : {id} deleted successfully.", product.Id);
            return true;
        }

        public List<Product> GetProducts()
        {
            _logger.LogInformation("Retrieving products from the database.");

            List<Product> products = _db.Products.Where(x => x.IsDeleted == false).ToList();
            _logger.LogInformation("Successfully retrieved {UserCount} products.", products.Count);
            return products;
        }
    }
}
