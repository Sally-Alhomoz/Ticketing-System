using Microsoft.AspNetCore.Http;
using SharedDTOs;

namespace TicketingSystem.Services.Interfaces
{
    public interface IAttachmentService
    {
        Task AddAttachments(List<IFormFile> files, Guid userId, Guid ticketId, Guid? commentId = null);
        Task<AttachmentDto?> GetAttachmentById(Guid id);
        Task<List<AttachmentDto>> GetByTicketId(Guid ticketId);
        Task<List<AttachmentDto>> GetByCommentId(Guid commentId);
    }
}
