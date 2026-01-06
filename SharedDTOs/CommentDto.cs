using Microsoft.AspNetCore.Http;

namespace SharedDTOs
{
    public class CommentDto
    {
        public Guid Id { get; set; }
        public string? Message { get; set; }

        public DateTime CreateDate { get; set; }
        public Guid CreatedBy { get; set; }
        public string CreatedByFullName { get; set; }
        public Guid TicketId { get; set; }
        public List<AttachmentDto> Attachments { get; set; } = new();
    }
}
