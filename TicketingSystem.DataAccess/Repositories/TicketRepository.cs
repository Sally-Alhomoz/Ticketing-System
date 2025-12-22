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

            ticket.IsDeleted = true;
            _logger.LogInformation("ticket with id : {id} deleted successfully.", ticket.Id);
            return true;
        }

        public List<Ticket> GetTicktes()
        {
            _logger.LogInformation("Retrieving tickets from the database.");

            List<Ticket> tickets = _db.Tickets.Where(x => x.IsDeleted == false).ToList();
            _logger.LogInformation("Successfully retrieved {UserCount} tickets.", tickets.Count);
            return tickets;
        }

        public bool TicketAssignedTo(Guid ticketId, Guid userid)
        {
            _logger.LogInformation("Assigning ticket to staff.");

            var ticket = _db.Tickets.FirstOrDefault(x => x.Id == ticketId);

            if(ticket == null)
            {
                _logger.LogWarning("Ticket not found.");
                return false;
            }

            if(ticket.AssignedTo == null)
            {
                ticket.AssignedTo = userid;
                _logger.LogInformation("Ticket assiged successfully.");
                return true;
            }

            _logger.LogWarning("Ticket already assiged.");
            return false;
        }

        public bool UpdateStatus(Guid id,TicketStatus newStatus)
        {
            _logger.LogInformation("Change ticket status.");

            var ticket = _db.Tickets.FirstOrDefault(x => x.Id == id);

            if (ticket == null)
            {
                _logger.LogWarning("Ticket not found.");
                return false;
            }

            ticket.Status = newStatus;
            _logger.LogInformation("Ticket status changed successfully.");
            return true;
        }

        public bool SetPriprity (Guid id , TicketPriority priority)
        {
            _logger.LogInformation("Set ticket priority.");

            var ticket = _db.Tickets.FirstOrDefault(x => x.Id == id);

            if (ticket == null)
            {
                _logger.LogWarning("Ticket not found.");
                return false;
            }

            ticket.Priority = priority;
            _logger.LogInformation("Ticket priority set successfully.");
            return true;

        }
    }
}
