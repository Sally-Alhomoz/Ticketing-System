using TicketingSystem.DataAccess.Interfaces;

namespace TicketingSystem.DataAccess.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        ITicketRepository Tickets { get; }
        IProductRepository Products { get; }
        ICommentRepository Comments { get;}
        IAttachmentRepository Attachments { get; }
        ITicketHistoryRepository TicketsHistory { get; }
        int Complete();
    }
}
