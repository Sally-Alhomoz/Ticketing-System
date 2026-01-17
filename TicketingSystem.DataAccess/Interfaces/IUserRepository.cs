using TicketingSystem.DataAccess.Models;

namespace TicketingSystem.DataAccess.Interfaces
{
    public interface IUserRepository
    {
        void Add(User user);
        IQueryable<User> GetUsers();
        void Delete(User user);
        Task<bool> VerifyPassword(string pass, Guid id, string storedhash);
        public string HashPassword(string pass, string id);
        Task<User?> GetByUsername(string name);
        void Update(User user);
    }
}
