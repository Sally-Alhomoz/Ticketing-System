using Microsoft.AspNetCore.Http;

namespace SharedDTOs
{
    public class CreateCommentDto
    {
        public string? Message { get; set; }
        public Guid TicketId { get; set; }
        public List<IFormFile>? Files { get; set; }
    }
}
