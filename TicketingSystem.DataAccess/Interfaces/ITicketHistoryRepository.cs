

using TicketingSystem.DataAccess.Models;

namespace TicketingSystem.DataAccess.Interfaces
{
    public interface ITicketHistoryRepository
    {
        void Add(TicketHistory record);
        Task<List<TicketHistory>> GetTicketHistoryByTicketId(Guid ticketId);
        Task<TicketHistory?> GetLatestForTicket(Guid ticketId);
    }
}
