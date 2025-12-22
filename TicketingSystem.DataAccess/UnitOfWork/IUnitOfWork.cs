using TicketingSystem.DataAccess.Interfaces;

namespace TicketingSystem.DataAccess.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        ITicketRepository Tickets { get; }
        IProductRepository Products { get; }
        int Complete();
    }
}
