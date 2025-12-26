

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using TicketingSystem.DataAccess.Interfaces;
using TicketingSystem.DataAccess.Models;

namespace TicketingSystem.DataAccess.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly TicketingSystemDBContext _db;
        private readonly ILogger<CommentRepository> _logger;

        public CommentRepository(TicketingSystemDBContext db, ILogger<CommentRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public void Add(Comment comment)
        {
            _logger.LogInformation("Adding a comment to the database.");
            _db.Comments.Add(comment);
            _logger.LogInformation("Comment added successfully");
        }

        public IQueryable<Comment> GetCommentsByTicketId(Guid ticketId)
        {
            _logger.LogInformation("Fetching comments for Ticket: {TicketId}", ticketId);

            var comments = _db.Comments
                .Include(c=>c.CreatedByUser)
                .Include(c=>c.Ticket)
                .Where(c => c.TicketId == ticketId);

            return comments;
        }

        public IQueryable<Comment> GetCommentsByUserId(Guid userId)
        {
            _logger.LogInformation("Fetching comments for User {userId}", userId);

            var comments = _db.Comments
                .Include(c => c.CreatedByUser)
                .Include(c => c.Ticket)
                .Where(c => c.CreatedBy == userId);

            return comments;
        }
    }
}
