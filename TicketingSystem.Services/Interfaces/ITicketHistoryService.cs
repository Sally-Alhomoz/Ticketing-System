using SharedDTOs;
using SharedDTOs.Enum;

namespace TicketingSystem.Services.Interfaces
{
    public interface ITicketHistoryService
    {
        Task AddRecord(Guid ticketId, TicketStatus newStatus, Guid userId);
        Task<List<TicketHistoryDto>?> GetTicketHistoryById(Guid ticketId);
        Task<TicketHistoryDto?> GetLatestForTicket(Guid ticketId);
    }
}
