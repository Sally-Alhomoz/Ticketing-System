using Microsoft.Extensions.Logging;
using SharedDTOs;
using SharedDTOs.Enum;
using System.Threading.Tasks;
using TicketingSystem.DataAccess.Models;
using TicketingSystem.DataAccess.UnitOfWork;
using TicketingSystem.Services.Interfaces;

namespace TicketingSystem.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<UserService> _logger;

        public UserService(IUnitOfWork uow,ILogger<UserService> logger)
        {
            _uow = uow;
            _logger = logger;   
        }

        public async Task Add(NewUserDto u)
        {
            _logger.LogInformation("Adding user");

            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = u.FirstName,
                LastName=u.LastName,
                Username = u.Username,
                Email = u.Email,
                Password = u.Password,
                Role = Role.Customer,
                Status = UserStatus.inActive
            };

            _uow.Users.Add(user);
            _logger.LogInformation("User added to the repository");
            await _uow.Complete();
        }

        public async Task<bool> Validatelogin(LoginDto user)
        {
            _logger.LogInformation("Validating login for username: {Username}", user.Username);

            var exist =await _uow.Users.GetByUsername(user.Username);
            if (exist == null)
            {
                _logger.LogWarning("Login failed. Username not found: {Username}", user.Username);
                return false;
            }

            bool flag = await VerifyPassword(user.Password, exist.Id, exist.Password);
            if (flag)
            {
                exist.Status = UserStatus.Active;
                _uow.Users.Update(exist);
                _logger.LogInformation("Login successful for username: {Username}", user.Username);
            }
            else
            {
                _logger.LogWarning("Login failed for username: {Username}", user.Username);
            }
            await _uow.Complete();
            return flag;

        }

        public async Task<bool> VerifyPassword(string pass, Guid id, string storedhash)
        {
            _logger.LogInformation("Verifying password for user ID {UserId}.", id);
            var hashed =await _uow.Users.VerifyPassword(pass, id, storedhash);

            if (hashed)
            {
                _logger.LogInformation("Password Verified successfully");
                return true;
            }
            _logger.LogWarning("Verifying password Failed");
            return false;
        }


        public async Task<bool> Delete(string username)
        {
            _logger.LogInformation("Deleting a user.");

            var flag = await _uow.Users.Delete(username);

            if (flag)
            {
                await _uow.Complete();
                _logger.LogInformation("Deleting a user with username : {Username} successfully.", username);
                return true;
            }

            _logger.LogInformation("Deleting a user with username : {Username} Failed.", username);
            return false;
        }

        public async Task<(List<UserDto> users, int totalCount)> GetUsersPaged(
           int page = 1,
           int pageSize = 10,
           string search = "",
           string sortBy = "username",
           string sortDirection = "asc")
        {
            var query = _uow.Users.GetUsers().AsQueryable();

            // GLOBAL SEARCH — all fields
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                query = query.Where(u =>
                    u.Username.ToLower().Contains(s) ||
                    (u.Email != null && u.Email.ToLower().Contains(s)) ||
                    u.Role.ToString().ToLower().Contains(s)
                );
            }

            // FULL SORTING — every column
            query = (sortBy?.ToLower(), sortDirection?.ToLower()) switch
            {
                ("email", "desc") => query.OrderByDescending(u => u.Email ?? ""),
                ("email", _) => query.OrderBy(u => u.Email ?? ""),
                ("role", "desc") => query.OrderByDescending(u => u.Role),
                ("role", _) => query.OrderBy(u => u.Role),
                ("status", "desc") => query.OrderByDescending(u => u.Status),
                ("status", _) => query.OrderBy(u => u.Status),
                (_, "desc") => query.OrderByDescending(u => u.Username),
                _ => query.OrderBy(u => u.Username)
            };

            int totalCount = query.Count();

            var users = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName=u.LastName,
                    Username = u.Username,
                    Email = u.Email,
                    Role = u.Role,
                    Status = u.Status
                })
                .ToList();

            return (users, totalCount);
        }

    }
}
