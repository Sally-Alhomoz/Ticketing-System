using SharedDTOs;

namespace TicketingSystem.Services.Interfaces
{
    public interface ITicketHistoryService
    {
        Task AddRecord(TicketHistoryDto dto);
        Task<List<TicketHistoryDto>?> GetTicketHistoryById(Guid ticketId);
        Task<TicketHistoryDto?> GetLatestForTicket(Guid ticketId);
    }
}
