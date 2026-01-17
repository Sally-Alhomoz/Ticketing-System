using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text;
using TicketingSystem.DataAccess.Interfaces;
using TicketingSystem.DataAccess.Models;

namespace TicketingSystem.DataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly TicketingSystemDBContext _db;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(TicketingSystemDBContext db , ILogger<UserRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public void Add(User user)
        {
            _logger.LogInformation("Adding user to the Database");

            _db.Users.Add(user);

            _logger.LogInformation("User added to the database successfully.");
        }

        public IQueryable<User> GetUsers()
        {
            _logger.LogInformation("Retrieving users from the database.");

            var users = _db.Users.AsNoTracking();
            _logger.LogInformation("Successfully retrieved users.");
            return users;
        }

        public void Delete(User user)
        {
            _logger.LogInformation("Deleting a user from the database.");

            _db.Users.Remove(user);
            _logger.LogInformation("User with username : {Username} deleted successfully.", user.Username);
        }

        public async Task<User?> GetByUsername(string name)
        {
            _logger.LogInformation("Retriving a user by username: {Username}.", name);

            var user =await _db.Users.FirstOrDefaultAsync(x => x.Username == name);
            if (user != null)
            {
                _logger.LogInformation("User with username : {Username} found.", name);
            }
            else
            {
                _logger.LogWarning("No user Found");
            }
            return user;
        }

        public void Update(User user)
        {
            _db.Users.Update(user);
        }

        public string HashPassword(string pass, string id)
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

        public async Task<bool> VerifyPassword(string pass, Guid id, string storedhash)
        {
            _logger.LogDebug("Verifying password for user.");

            var hashed = HashPassword(pass, id.ToString());

            bool result = (hashed == storedhash);


            _logger.LogDebug("Verifying password for user : {result}",result);
            return result;
        }
    }
}
