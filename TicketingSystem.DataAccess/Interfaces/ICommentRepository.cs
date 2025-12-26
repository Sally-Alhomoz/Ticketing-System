
using TicketingSystem.DataAccess.Models;

namespace TicketingSystem.DataAccess.Interfaces
{
    public interface ICommentRepository
    {
        void Add(Comment comment);
        IQueryable<Comment> GetCommentsByTicketId(Guid ticketId);
        IQueryable<Comment> GetCommentsByUserId(Guid userId);
    }
}
