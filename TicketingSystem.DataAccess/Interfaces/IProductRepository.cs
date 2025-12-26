using TicketingSystem.DataAccess.Models;

namespace TicketingSystem.DataAccess.Interfaces
{
    public interface IProductRepository
    {
        void Add(Product product);
        Task <bool> Delete(int id);
        IQueryable<Product> GetProducts();
    }
}
