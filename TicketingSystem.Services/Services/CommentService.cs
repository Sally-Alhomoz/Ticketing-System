using Microsoft.Extensions.Logging;
using SharedDTOs;
using TicketingSystem.DataAccess.UnitOfWork;
using TicketingSystem.Services.Interfaces;
using TicketingSystem.DataAccess.Models;

namespace TicketingSystem.Services.Services
{
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<CommentService> _logger;
        public CommentService(IUnitOfWork uow, ILogger<CommentService> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task Add(CommentDto dto)
        {
            _logger.LogInformation("Adding a comment.");

            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                Message = dto.Message,
                CreateDate = DateTime.UtcNow,
                CreatedBy = dto.CreatedBy,
                TicketId = dto.TicketId
            };

            _uow.Comments.Add(comment);
            await _uow.Complete();
            _logger.LogInformation("Comment added successfully.");
        }

        public async Task<List<CommentDto>> GetCommentsByTicketId(Guid ticketId)
        {
            _logger.LogInformation("Retrieving commnet history for Ticket {TicketId}", ticketId);

            var comments = _uow.Comments.GetCommentsByTicketId(ticketId).OrderBy(c => c.CreateDate);

            var dto = comments.Select(c => new CommentDto
            {
                Id = c.Id,
                Message = c.Message,
                CreateDate = c.CreateDate,
                TicketId = c.TicketId,
                CreatedBy = c.CreatedBy,
                CreatedByFullName = c.CreatedByUser.FirstName + " " + c.CreatedByUser.LastName
            }).ToList();

            if (dto.Count == 0)
            {
                _logger.LogInformation("No comments found for Ticket {TicketId}", ticketId);
            }

            _logger.LogInformation("Comments retrieved successfully.");
            return dto;
        }

        public async Task<List<CommentDto>> GetCommentsByUserId(Guid userId)
        {
            _logger.LogInformation("Retrieving commnet history for user {UserId}", userId);

            var comments = _uow.Comments.GetCommentsByUserId(userId).OrderBy(c => c.CreateDate);


            var dto = comments.Select(c => new CommentDto
            {
                Id = c.Id,
                Message = c.Message,
                CreateDate = c.CreateDate,
                TicketId = c.TicketId,
                CreatedBy = c.CreatedBy,
                CreatedByFullName = c.CreatedByUser.FirstName + " " + c.CreatedByUser.LastName
            }).ToList();

            if (dto.Count == 0 )
            {
                _logger.LogWarning("No comments found for Ticket {UserId}", userId);
            }

            _logger.LogInformation("Comments retrieved successfully.");
            return dto;
        }
    }
}
