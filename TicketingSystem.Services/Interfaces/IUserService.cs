using SharedDTOs;

namespace TicketingSystem.Services.Interfaces
{
    public interface IUserService
    {
        void Add(NewUserDto u);
        bool Validatelogin(LoginDto user);
        bool VerifyPassword(string pass, Guid id, string storedhash);
        string Delete(string username);
        (List<UserDto> users, int totalCount) GetUsersPaged(int page, int pageSize, string search, string sortBy, string sortDirection);
    }
}
