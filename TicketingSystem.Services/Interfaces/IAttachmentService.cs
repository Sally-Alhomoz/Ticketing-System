using SharedDTOs;

namespace TicketingSystem.Services.Interfaces
{
    public interface IAttachmentService
    {
        Task AddAttachment(AttachmentDto dto);
        Task<AttachmentDto?> GetAttachmentById(Guid id);
        Task<List<AttachmentDto>> GetByTicketId(Guid ticketId);
    }
}
