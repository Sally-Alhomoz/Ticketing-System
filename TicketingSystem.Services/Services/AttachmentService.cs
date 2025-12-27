using Microsoft.Extensions.Logging;
using TicketingSystem.DataAccess.UnitOfWork;
using TicketingSystem.Services.Interfaces;
using SharedDTOs;
using TicketingSystem.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace TicketingSystem.Services.Services
{
    public class AttachmentService :IAttachmentService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<AttachmentService> _logger;
        public AttachmentService(IUnitOfWork uow, ILogger<AttachmentService> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task AddAttachment(AttachmentDto dto)
        {
            _logger.LogInformation("Adding an attachment.");

            var attachment = new Attachment
            {
                Id = Guid.NewGuid(),
                UploadedBy = dto.UploadedBy,
                FileName = dto.FileName,
                FilePath = dto.FilePath,
                CreateDate=DateTime.UtcNow,
                TicketId = dto.TicketId,
                CommentId = dto.CommentId
            };

            _uow.Attachments.Add(attachment);
            await _uow.Complete();
            _logger.LogInformation("Attachment added successfully.");
        }

        public async Task<AttachmentDto?> GetAttachmentById(Guid id)
        {
            _logger.LogInformation("Retrieving attachemnt with id :{Id}", id);

            var attachmen = await _uow.Attachments.GetAttachmentById(id);

            if(attachmen == null)
            {
                _logger.LogWarning("No attachment found.");
                return null;
            }

            var dto = new AttachmentDto
            {
                Id = attachmen.Id,
                CreateDate = attachmen.CreateDate,
                FileName = attachmen.FileName,
                FilePath = attachmen.FilePath,
                CommentId = attachmen.CommentId,
                TicketId = attachmen.TicketId,
                UploadedBy = attachmen.UploadedBy,
                UploadedByFullName = attachmen.UploadedByUser.FirstName + " " + attachmen.UploadedByUser.LastName
            };

            _logger.LogInformation("Attachment retrieved successfully.");
            return dto;
        }

        public async Task<List<AttachmentDto>> GetByTicketId(Guid ticketId)
        {
            _logger.LogInformation("Retrieving attachemnts for user {TicketId}", ticketId);

            var attachmens = _uow.Attachments.GetByTicketId(ticketId).OrderBy(a => a.CreateDate);

            var dto = attachmens.Select(a => new AttachmentDto
            {
                Id = a.Id,
                FileName = a.FileName,
                FilePath = a.FilePath,
                TicketId = a.TicketId,
                CreateDate=a.CreateDate,
                UploadedBy = a.UploadedBy,
                UploadedByFullName = a.UploadedByUser.FirstName + " " + a.UploadedByUser.LastName,
                CommentId = a.CommentId
            }).ToList();

            if(dto.Count == 0)
            {
                _logger.LogWarning("No comments found for Ticket");
            }

            return dto;
        }
    }
}
