using TicketingSystem.DataAccess.Models;

namespace TicketingSystem.DataAccess.Interfaces
{
    public interface IUserRepository
    {
        bool Add(User user);
        List<User> GetUsers();
        bool Delete(string username);
        bool VerifyPassword(string pass, Guid id, string storedhash);
    }
}
