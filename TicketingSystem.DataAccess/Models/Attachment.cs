using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [ForeignKey("UploadedBy")]
        public User UploadedByUser { get; set; }
        public Guid TicketId { get; set; }
        [ForeignKey("TicketId")]
        public Ticket Ticket { get; set; }
        public Guid? CommentId { get; set; }
    }
}
