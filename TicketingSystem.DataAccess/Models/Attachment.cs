using System.ComponentModel.DataAnnotations;

namespace TicketingSystem.DataAccess.Models
{
    public class Attachment
    {
        [Key]
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime CreateDate { get; set; }
        public Guid UploadedBy { get; set; }
        public User UploadedByUser { get; set; }
        public Guid TicketId { get; set; }
        public Guid? CommentId { get; set; }
    }
}
