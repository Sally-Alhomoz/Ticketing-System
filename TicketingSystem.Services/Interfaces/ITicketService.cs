using SharedDTOs;
using SharedDTOs.Enum;

namespace TicketingSystem.Services.Interfaces
{
    public interface ITicketService
    {
        Task Add(TicketDto t);
        Task<bool> AssignTicket(Guid ticketId, Guid userid);
        Task<bool> UpdateTicketStatus(Guid ticketId, TicketStatus newStatus, Guid userId);
        Task<TicketDto?> GetTicketById(Guid ticketId);
        Task<(List<TicketDto> ticktes, int totalCount)> GetTicketsPaged(int page, int pageSize, string search, string sortBy, string sortDirection);
    }
}
