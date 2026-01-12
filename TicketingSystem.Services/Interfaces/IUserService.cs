using SharedDTOs;

namespace TicketingSystem.Services.Interfaces
{
    public interface IUserService
    {
        Task Add(NewUserDto u);
        Task<string> AddStaff(NewUserDto u);
        Task<bool> Validatelogin(LoginDto user);
        Task<bool> VerifyPassword(string pass, Guid id, string storedhash);
        Task<bool> Delete(string username);
        Task<UserDto?> GetUserByUsername(string username);
        Task<bool> SetUserInActive(string username);
        Task<bool> UpdateProfile(string username, UpdateProfileDto updateUserData);
        Task<List<UserDto>> GetStaff();
        Task<bool> ChangePassword(string username, ChangePassword dto);
        Task<(List<UserDto> users, int totalCount)> GetUsersPaged(int page, int pageSize, string search, string sortBy, string sortDirection);
    }
}
