using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SharedDTOs;
using TicketingSystem.DataAccess.Models;
using TicketingSystem.DataAccess.UnitOfWork;
using TicketingSystem.Services.Interfaces;
using Attachment = TicketingSystem.DataAccess.Models.Attachment;

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

        private async Task<string> SaveFileAsync(IFormFile file)
        {
            var rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(rootPath)) Directory.CreateDirectory(rootPath);

            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var fullPath = Path.Combine(rootPath, uniqueFileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fullPath;
        }

        public async Task AddAttachments(List<IFormFile> files, Guid userId, Guid ticketId, Guid? commentId = null)
        {
            foreach (var file in files)
            {
                var physicalPath = await SaveFileAsync(file);

                var attachment = new Attachment
                {
                    Id = Guid.NewGuid(),
                    FileName = file.FileName,
                    FilePath = physicalPath,
                    CreateDate = DateTime.Now,
                    UploadedBy = userId,
                    TicketId = ticketId,
                    CommentId = commentId 
                };

                _uow.Attachments.Add(attachment);
            }
        }


        public async Task<AttachmentDto?> GetAttachmentById(Guid id)
        {
            _logger.LogInformation("Retrieving attachemnt with id :{Id}", id);

            var attachment = await _uow.Attachments.GetAttachmentById(id);

            if(attachment == null)
            {
                _logger.LogWarning("No attachment found.");
                return null;
            }

            var dto = new AttachmentDto
            {
                Id = attachment.Id,
                CreateDate = attachment.CreateDate,
                FilePath=attachment.FilePath,
                FileName = attachment.FileName,
                FileUrl = $"/api/Attachments/Download/{attachment.Id}",
                CommentId = attachment.CommentId,
                TicketId = attachment.TicketId,
                UploadedBy = attachment.UploadedBy,
                UploadedByFullName = attachment.UploadedByUser.FirstName + " " + attachment.UploadedByUser.LastName
            };

            _logger.LogInformation("Attachment retrieved successfully.");
            return dto;
        }

        public async Task<List<AttachmentDto>> GetByTicketId(Guid ticketId)
        {
            _logger.LogInformation("Retrieving attachemnts for ticket {TicketId}", ticketId);

            var attachmens = _uow.Attachments.GetByTicketId(ticketId).OrderBy(a => a.CreateDate);

            var dto = attachmens.Select(a => new AttachmentDto
            {
                Id = a.Id,
                FileName = a.FileName,
                FilePath=a.FilePath,
                FileUrl = $"/api/Attachments/Download/{a.Id}",
                TicketId = a.TicketId,
                CreateDate=a.CreateDate,
                UploadedBy = a.UploadedBy,
                UploadedByFullName = a.UploadedByUser.FirstName + " " + a.UploadedByUser.LastName,
                CommentId = a.CommentId
            }).ToList();

            if(dto.Count == 0)
            {
                _logger.LogWarning("No attachments found for Ticket");
            }

            return dto;
        }

        public async Task<List<AttachmentDto>> GetByCommentId(Guid commentId)
        {
            _logger.LogInformation("Retrieving attachemnts for user ");

            var attachmens = _uow.Attachments.GetByCommentId(commentId).OrderBy(a => a.CreateDate);

            var dto = attachmens.Select(a => new AttachmentDto
            {
                Id = a.Id,
                FileName = a.FileName,
                FilePath = a.FilePath,
                FileUrl = $"/api/Attachments/Download/{a.Id}",
                TicketId = a.TicketId,
                CreateDate = a.CreateDate,
                UploadedBy = a.UploadedBy,
                UploadedByFullName = a.UploadedByUser.FirstName + " " + a.UploadedByUser.LastName,
                CommentId = a.CommentId
            }).ToList();

            if (dto.Count == 0)
            {
                _logger.LogWarning("No attachments found for this comment.");
            }

            return dto;
        }
    }
}
