
using TicketingSystem.DataAccess.Models;

namespace TicketingSystem.DataAccess.Interfaces
{
    public interface ICommentRepository
    {
        void Add(Comment comment);
        List<Comment> GetCommentsByTicketId(Guid ticketId);
        List<Comment> GetCommentsByUserId(Guid userId);
    }
}
