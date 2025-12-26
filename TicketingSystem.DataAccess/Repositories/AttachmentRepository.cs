using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TicketingSystem.DataAccess.Interfaces;
using TicketingSystem.DataAccess.Models;

namespace TicketingSystem.DataAccess.Repositories
{
    public class AttachmentRepository : IAttachmentRepository
    {
        private readonly TicketingSystemDBContext _db;
        private readonly ILogger<AttachmentRepository> _logger;

        public AttachmentRepository(TicketingSystemDBContext db, ILogger<AttachmentRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public void Add(Attachment attachment)
        {
            _logger.LogInformation("Adding an attachment to the database.");
            _db.Attachments.Add(attachment);
            _logger.LogInformation("Attachment added successfully");
        }

        public IQueryable<Attachment> GetByTicketId(Guid ticketId)
        {
            _logger.LogInformation("Retrieving attachments for Ticket {TicketId}", ticketId);
            return _db.Attachments.Where(a => a.TicketId == ticketId);
        }
    }
}
