using TicketingSystem.DataAccess.Models;

namespace TicketingSystem.DataAccess.Interfaces
{
    public interface IAttachmentRepository
    {
        void Add(Attachment attachment);
        Task<Attachment> GetAttachmentById(Guid id);
        IQueryable<Attachment> GetByTicketId(Guid ticketId);
    }
}
