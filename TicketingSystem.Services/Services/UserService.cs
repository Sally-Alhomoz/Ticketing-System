using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Logging;
using SharedDTOs;
using SharedDTOs.Enum;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
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

        public async Task<string> AddStaff(NewUserDto u)
        {
            _logger.LogInformation("Adding new support staff."); 

            string tempPassword = Guid.NewGuid().ToString("N").Substring(0, 8); 

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = u.Username,
                FirstName=u.FirstName,
                LastName=u.LastName,
                Email=u.Email,
                Password = tempPassword, 
                Role = Role.Support,
                Status = UserStatus.Pending 
            };

            _uow.Users.Add(user);
            await _uow.Complete();
            _logger.LogInformation("Staff added to the repository");
            return tempPassword;
        }

        public async Task<UserDto?> GetUserByUsername(string username)
        {
            _logger.LogInformation("Retriving a user by username: {Username}.", username);

            var user = await _uow.Users.GetByUsername(username);

            if(user == null)
            {
                _logger.LogWarning("No user found with username: {Username}", username);
                return null;
            }

            var dto = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Status = user.Status,
                Email = user.Email,
                Role = user.Role
            };

            _logger.LogInformation("User found successfully.");
            return dto;

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
                if(exist.Status != UserStatus.Pending)
                {
                    exist.Status = UserStatus.Active;
                    _uow.Users.Update(exist);
                }
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

        public async Task<bool> SetUserInActive(string username)
        {
            _logger.LogInformation("Setting status for user {Username} to inactive", username);

            var user = await _uow.Users.GetByUsername(username);

            if(user ==null)
            {
                _logger.LogWarning("No user found with username: {Username}", username);
                return false;
            }

            user.Status = UserStatus.inActive;
            _uow.Users.Update(user);
            await _uow.Complete();

            _logger.LogInformation("User status updated successfully for {Username}", username);
            return true;
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

        public async Task<List<UserDto>> GetStaff()
        {
            _logger.LogInformation("Get all staff employess");

            var staff = _uow.Users.GetUsers()
                .Where(u => u.Role == Role.Support || u.Role == Role.Admin)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Username = u.Username,
                    Email = u.Email,
                    Role = u.Role,
                    Status = u.Status
                })
                .ToList();

            return staff;
        }

        private string HashPassword(string pass, string id)
        {
            _logger.LogDebug("Hashing password for user ID {UserId}.", id);
            byte[] userid = Encoding.UTF8.GetBytes(id);

            byte[] hashed = KeyDerivation.Pbkdf2(
                password: pass,
                salt: userid,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 32);

            return Convert.ToBase64String(hashed);
        }

        public async Task<bool> ChangePassword(string username, ChangePassword dto)
        {
            _logger.LogInformation("Attempting to cgange passord for user: {Username}", username);

            var user = await _uow.Users.GetByUsername(username);

            if (user == null )
            {
                _logger.LogWarning("User not found.");
                return false;
            }

            bool isCurrentValid = await _uow.Users.VerifyPassword(dto.CurrentPassword, user.Id, user.Password);

            if (!isCurrentValid)
            {
                _logger.LogWarning("Verification failed for {Username}.", username);
                return false;
            }
            user.Password = HashPassword(dto.NewPassword, user.Id.ToString());

            if (user.Status == UserStatus.Pending)
            {
                user.Status = UserStatus.Active;
                _logger.LogInformation("User {Username} status promoted from Pending to Active.", username);
            }

            _uow.Users.Update(user);
            await _uow.Complete();

            _logger.LogInformation("Password changed successfully for {Username}", username);
            return true;
        }

        public async Task<bool> UpdateProfile(string username, UpdateProfileDto updateUserData)
        {
            _logger.LogInformation("Updating profile for user: {Username}", username);

            var user = await _uow.Users.GetByUsername(username);

            if (user == null)
            {
                _logger.LogWarning("Update failed. User {Username} not found.", username);
                return false;
            }

            user.FirstName = updateUserData.FirstName;
            user.LastName = updateUserData.LastName;
            user.Email = updateUserData.Email;

            _uow.Users.Update(user);
            await _uow.Complete();

            _logger.LogInformation("Profile updated successfully for {Username}", username);
            return true;
        }


        public async Task<(List<UserDto> users, int totalCount)> GetUsersPaged(
           int page = 1,
           int pageSize = 10,
           string search = "",
           string sortBy = "username",
           string sortDirection = "asc")
        {
            var query = _uow.Users.GetUsers().AsQueryable();

            // 1. GLOBAL SEARCH
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();

                bool searchMatchesAdmin = "admin".Contains(s);
                bool searchMatchesCustomer = "customer".Contains(s);
                bool searchMatchesSupport = "support".Contains(s);
                bool searchMatchesActive = "active".Contains(s);
                bool searchMatchesInactive = "inactive".Contains(s);

                query = query.Where(u =>
                    u.Username.ToLower().Contains(s) ||
                    (u.Email != null && u.Email.ToLower().Contains(s)) ||
                    u.FirstName.ToLower().Contains(s) ||
                    u.LastName.ToLower().Contains(s) ||
                    (searchMatchesAdmin && u.Role == Role.Admin) ||
                    (searchMatchesCustomer && u.Role == Role.Customer) ||
                    (searchMatchesSupport && u.Role == Role.Support)||
                    (searchMatchesActive && u.Status == UserStatus.Active) ||
                    (searchMatchesInactive && u.Status == UserStatus.inActive)
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
                ("firstName", "desc") => query.OrderByDescending(u => u.FirstName),
                ("firstame", _) => query.OrderBy(u => u.FirstName),
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
