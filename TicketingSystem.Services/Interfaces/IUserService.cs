using SharedDTOs;

namespace TicketingSystem.Services.Interfaces
{
    public interface IUserService
    {
        Task Add(NewUserDto u);
        Task<bool> Validatelogin(LoginDto user);
        Task<bool> VerifyPassword(string pass, Guid id, string storedhash);
        Task<bool> Delete(string username);
        Task<(List<UserDto> users, int totalCount)> GetUsersPaged(int page, int pageSize, string search, string sortBy, string sortDirection);
    }
}
