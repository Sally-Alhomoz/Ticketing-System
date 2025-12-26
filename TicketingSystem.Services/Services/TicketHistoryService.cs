using Microsoft.Extensions.Logging;
using SharedDTOs;
using TicketingSystem.DataAccess.Models;
using TicketingSystem.DataAccess.UnitOfWork;
using TicketingSystem.Services.Interfaces;

namespace TicketingSystem.Services.Services
{
    public class TicketHistoryService : ITicketHistoryService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<TicketHistoryService> _logger;

        public TicketHistoryService(IUnitOfWork uow, ILogger<TicketHistoryService> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task AddRecord(TicketHistoryDto dto)
        {
            _logger.LogInformation("Adding a ticket record.");

            var record = new TicketHistory
            {
                Id = dto.Id,
                ChangeDate = DateTime.UtcNow,
                NewStatus = dto.NewStatus,
                ChangedBy = dto.ChangedBy,
                TicketId = dto.TicketId
            };

            _uow.TicketsHistory.Add(record);
            _logger.LogInformation("Ticket record added successfully");
            _uow.Complete();
        }

        public async Task<List<TicketHistoryDto>?> GetTicketHistoryById(Guid ticketId)
        {
            _logger.LogInformation("Fetching ticket history records.");

            var records = await _uow.TicketsHistory.GetTicketHistoryByTicketId(ticketId);

            if(records ==null)
            {
                _logger.LogWarning("No records fiund for this ticket.");
                return null;
            }

            var historyDtos = records.Select(h => new TicketHistoryDto
            {
                Id = h.Id,
                TicketId = h.TicketId,
                TicketTitle = h.Ticket.Title,
                ChangeDate = h.ChangeDate,
                ChangedBy = h.ChangedBy,
                ChangedByFullName = h.ChangedByUser.FirstName + " " + h.ChangedByUser.LastName,
                NewStatus = h.NewStatus
            }).ToList();

            _logger.LogInformation("Successfully returned history records.");

            return historyDtos;
        }

        public async Task<TicketHistoryDto?> GetLatestForTicket(Guid ticketId)
        {
            _logger.LogInformation("Fetching recent ticket history record.");

            var record = await _uow.TicketsHistory.GetLatestForTicket(ticketId);

            if(record ==null)
            {
                _logger.LogWarning("No records fiund for this ticket.");
                return null;
            }

            var dto = new TicketHistoryDto
            {
                Id = record.Id,
                TicketId = record.TicketId,
                TicketTitle = record.Ticket.Title,
                ChangeDate = record.ChangeDate,
                ChangedByFullName = record.ChangedByUser.FirstName + " " + record.ChangedByUser.LastName,
                NewStatus = record.NewStatus,
                ChangedBy = record.ChangedBy
            };

            _logger.LogInformation("Successfully returned recent history record.");
            return dto;
        }
    }
}
