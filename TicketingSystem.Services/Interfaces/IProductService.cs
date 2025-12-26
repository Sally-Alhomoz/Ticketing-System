using SharedDTOs;
using System.Threading.Tasks;

namespace TicketingSystem.Services.Interfaces
{
    public interface IProductService
    {
        Task AddProduct(ProductDto dto);
        Task<bool> DeleteProduct(int productId);
        Task<(List<ProductDto> products, int totalCount)> GetProductPaged(int page, int pageSize, string search, string sortBy, string sortDirection);
    }
}
