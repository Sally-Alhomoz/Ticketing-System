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

        public async Task<Attachment> GetAttachmentById(Guid id)
        {
            _logger.LogInformation("Retrieving attachment with id {Id}", id);

            var attachment = _db.Attachments.FirstOrDefault(a => a.Id == id);

            return attachment;
        }

        public IQueryable<Attachment> GetByTicketId(Guid ticketId)
        {
            _logger.LogInformation("Retrieving attachments for Ticket {TicketId}", ticketId);
            return _db.Attachments
                .Include(a=> a.UploadedByUser)
                .Where(a => a.TicketId == ticketId);
        }
    }
}
