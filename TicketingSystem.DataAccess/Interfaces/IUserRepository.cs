using TicketingSystem.DataAccess.Models;

namespace TicketingSystem.DataAccess.Interfaces
{
    public interface IUserRepository
    {
        void Add(User user);
        IQueryable<User> GetUsers();
        Task<bool> Delete(string username);
        Task<bool> VerifyPassword(string pass, Guid id, string storedhash);
        Task<User?> GetByUsername(string name);
        void Update(User user);
    }
}
