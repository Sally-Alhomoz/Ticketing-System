using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedDTOs.Enum;
using TicketingSystem.DataAccess.Interfaces;
using TicketingSystem.DataAccess.Models;

namespace TicketingSystem.DataAccess.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly TicketingSystemDBContext _db;
        private readonly ILogger<TicketRepository> _logger;

        public TicketRepository(TicketingSystemDBContext db, ILogger<TicketRepository> logger)
        {
            _db = db;
            _logger = logger;
        }
        public void Add(Ticket ticket)
        {
            _logger.LogInformation("Adding a ticket to the database.");
            _db.Tickets.Add(ticket);
            _logger.LogInformation("Ticket added successfully");
        }

        public bool Delete(Guid id)
        {
            _logger.LogInformation("Deleting a ticket from the database.");

            var ticket= _db.Tickets.FirstOrDefault(x => x.Id == id);

            if (ticket == null)
            {
                _logger.LogWarning("ticket not found.");
                return false;
            }

            ticket.Status = TicketStatus.Deleted;
            _logger.LogInformation("ticket with id : {id} deleted successfully.", ticket.Id);
            return true;
        }

        public List<Ticket> GetTicktes()
        {
            _logger.LogInformation("Retrieving tickets from the database.");

            List<Ticket> tickets = _db.Tickets.Where(x => x.Status != TicketStatus.Deleted).ToList();
            _logger.LogInformation("Successfully retrieved {UserCount} tickets.", tickets.Count);
            return tickets;
        }

        public async Task<Ticket?> GetTicketById(Guid ticketId)
        {
            _logger.LogInformation("Retrive ticket by id.");

            var ticket = await _db.Tickets.FirstOrDefaultAsync(x => x.Id == ticketId);

            if(ticket == null)
            {
                _logger.LogWarning("Ticket not found.");
                return null;
            }

            _logger.LogWarning("Ticket found.");
            return ticket;
        }

        public void UpdateTicket(Ticket ticket)
        {
            _logger.LogInformation("Updating ticket.");
             _db.Tickets.Update(ticket);
        }
    }
}
