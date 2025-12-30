using SharedDTOs;

namespace TicketingSystem.Services.Interfaces
{
    public interface ICommentService
    {
        Task Add(CreateCommentDto dto, Guid userId);
        Task<List<CommentDto>> GetCommentsByTicketId(Guid ticketId);
        Task<List<CommentDto>> GetCommentsByUserId(Guid userId);
    }
}
