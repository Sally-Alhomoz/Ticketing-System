using SharedDTOs.Enum;
using TicketingSystem.DataAccess.Models;

namespace TicketingSystem.DataAccess.Interfaces
{
    public interface ITicketRepository
    {
        void Add(Ticket ticket);
        bool Delete(Guid id);
        List<Ticket> GetTicktes();
        Task<Ticket?> GetTicketById(Guid ticketId);
        void UpdateTicket(Ticket ticket);
    }
}
