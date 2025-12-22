using SharedDTOs.Enum;
using TicketingSystem.DataAccess.Models;

namespace TicketingSystem.DataAccess.Interfaces
{
    public interface ITicketRepository
    {
        void Add(Ticket ticket);
        bool Delete(Guid id);
        List<Ticket> GetTicktes();
        bool TicketAssignedTo(Guid ticketId, Guid userid);
        bool UpdateStatus(Guid id, TicketStatus newStatus);
        bool SetPriprity(Guid id, TicketPriority priority);
    }
}
