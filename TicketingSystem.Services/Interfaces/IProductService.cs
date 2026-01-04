using SharedDTOs;

namespace TicketingSystem.Services.Interfaces
{
    public interface IProductService
    {
        Task AddProduct(string name);
        Task<bool> DeleteProduct(int productId);
        Task<ProductDto?> GetProductByName(string name);
        Task<(List<ProductDto> products, int totalCount)> GetProductPaged(int page, int pageSize, string search, string sortBy, string sortDirection);
    }
}
