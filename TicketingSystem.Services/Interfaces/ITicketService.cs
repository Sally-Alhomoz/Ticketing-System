using SharedDTOs;
using SharedDTOs.Enum;

namespace TicketingSystem.Services.Interfaces
{
    public interface ITicketService
    {
        Task Add(CreateTicketDto t, Guid CreateById);
        Task<bool> AssignTicket(Guid ticketId, Guid userid);
        Task<bool> UpdateTicketStatus(Guid ticketId, TicketStatus newStatus, Guid userId);
        Task<TicketDto?> GetTicketById(Guid ticketId);
        Task<bool> SetPriprity(Guid ticketId, TicketPriority priority);
        Task<bool> DeleteTicket(Guid ticketId, Guid userId);
        Task<int> GetCustomerTicketCount(Guid userId);
        Task<int> GetStaffTicketCount(Guid userId);
        Task<int> GetStatusTicketCount(Guid userId, TicketStatus status);
        Task<int> GetUnAssignedTicketCount();
        Task<int> GetRsolvedTicketCount();
        Task<int> GetActiveTicketsCount(Guid userId, TicketStatus status);
        Task<(List<TicketDto> ticktes, int totalCount)> GetTicketsPaged(Guid currentUserId,bool isStaff,
            bool isAdmin, int page, int pageSize, string search, string sortBy, string sortDirection);
    }
}
