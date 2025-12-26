using SharedDTOs;

namespace TicketingSystem.Services.Interfaces
{
    public interface ICommentService
    {
        Task Add(CommentDto dto);
        Task<List<CommentDto>> GetCommentsByTicketId(Guid ticketId);
        Task<List<CommentDto>> GetCommentsByUserId(Guid userId);
    }
}
