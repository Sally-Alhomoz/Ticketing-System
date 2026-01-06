using Microsoft.Extensions.Logging;
using SharedDTOs;
using System.Net.Mail;
using System.Net.Sockets;
using TicketingSystem.DataAccess.Models;
using TicketingSystem.DataAccess.UnitOfWork;
using TicketingSystem.Services.Interfaces;

namespace TicketingSystem.Services.Services
{
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork _uow;
        private readonly IAttachmentService _attachmentService;
        private readonly ILogger<CommentService> _logger;
        public CommentService(IUnitOfWork uow, IAttachmentService attachmentService, ILogger<CommentService> logger)
        {
            _uow = uow;
            _logger = logger;
            _attachmentService = attachmentService;
        }

        public async Task Add(CreateCommentDto dto, Guid userId)
        {
            _logger.LogInformation("Adding a comment.");

            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                Message = dto.Message,
                CreateDate = DateTime.Now,
                CreatedBy = userId,
                TicketId = dto.TicketId
            };

            _uow.Comments.Add(comment);
            await _uow.Complete();

            if (dto.Files != null && dto.Files.Any())
            {
                await _attachmentService.AddAttachments(dto.Files, userId, dto.TicketId,comment.Id);
                await _uow.Complete();
            }

            _logger.LogInformation("Comment added successfully.");
        }

        public async Task<List<CommentDto>> GetCommentsByTicketId(Guid ticketId)
        {
            _logger.LogInformation("Retrieving commnet history for Ticket {TicketId}", ticketId);

            var comments = _uow.Comments.GetCommentsByTicketId(ticketId).OrderByDescending(c => c.CreateDate).ToList();

            var attachments = await _attachmentService.GetByTicketId(ticketId);

            var dto = comments.Select(c => new CommentDto
            {
                Id = c.Id,
                Message = c.Message,
                CreateDate = c.CreateDate,
                TicketId = c.TicketId,
                CreatedBy = c.CreatedBy,
                CreatedByFullName = c.CreatedByUser.FirstName + " " + c.CreatedByUser.LastName,
                Attachments = attachments.Where(a => a.CommentId == c.Id)
                .Select(a => new AttachmentDto
                {
                    Id = a.Id,
                    CreateDate = a.CreateDate,
                    FileName = a.FileName,
                    FileUrl = $"/api/Attachments/Download/{a.Id}",
                    UploadedBy = a.UploadedBy,
                    UploadedByFullName = a.UploadedByFullName,
                    TicketId = a.TicketId
                }).ToList()
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
            _logger.LogInformation("Retrieving comment history for user {UserId}", userId);

            var comments = _uow.Comments.GetCommentsByUserId(userId).OrderBy(c => c.CreateDate);

            var commentList = new List<CommentDto>();

            foreach (var c in comments)
            {
                var commentAttachments = await _attachmentService.GetByCommentId(c.Id);

                commentList.Add(new CommentDto
                {
                    Id = c.Id,
                    Message = c.Message,
                    CreateDate = c.CreateDate,
                    TicketId = c.TicketId,
                    CreatedBy = c.CreatedBy,
                    CreatedByFullName = c.CreatedByUser.FirstName + " " + c.CreatedByUser.LastName,
                    Attachments = commentAttachments 
                });
            }

            return commentList;
        }
    }
}
