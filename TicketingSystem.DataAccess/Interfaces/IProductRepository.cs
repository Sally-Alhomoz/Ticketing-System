using TicketingSystem.DataAccess.Models;

namespace TicketingSystem.DataAccess.Interfaces
{
    public interface IProductRepository
    {
        void Add(Product product);
        bool Delete(int id);
        List<Product> GetProducts();
    }
}
