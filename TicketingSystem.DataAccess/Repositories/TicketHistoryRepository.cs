using Microsoft.Extensions.Logging;
using System.Net.Sockets;
using TicketingSystem.DataAccess.Interfaces;
using TicketingSystem.DataAccess.Models;

namespace TicketingSystem.DataAccess.Repositories
{
    public class TicketHistoryRepository : ITicketHistoryRepository
    {
        private readonly TicketingSystemDBContext _db;
        private readonly ILogger<TicketHistoryRepository> _logger;

        public TicketHistoryRepository(TicketingSystemDBContext db, ILogger<TicketHistoryRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public void Add(TicketHistory record)
        {
            _logger.LogInformation("Adding a ticket record to the database.");
            _db.TicketsHistory.Add(record);
            _logger.LogInformation("Ticket record added successfully");
        }

        public List<TicketHistory> GetTicketHistoryByTicketId(Guid ticketId)
        {
            _logger.LogInformation("Fetching a ticket history record from the database.");
            var history = _db.TicketsHistory.Where(x => x.TicketId == ticketId).ToList();

            return history;
        }

        public TicketHistory GetLatestForTicket(Guid ticketId)
        {
            _logger.LogInformation("Fetching recent ticket history record from the database.");
            var history = _db.TicketsHistory.Where(x => x.TicketId == ticketId).OrderByDescending(h => h.ChangeDate).FirstOrDefault();
            return history;
        }
    }
}
