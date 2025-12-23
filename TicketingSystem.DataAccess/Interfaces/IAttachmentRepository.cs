using TicketingSystem.DataAccess.Models;

namespace TicketingSystem.DataAccess.Interfaces
{
    public interface IAttachmentRepository
    {
        void Add(Attachment attachment);
        List<Attachment> GetByTicketId(Guid ticketId);
    }
}
