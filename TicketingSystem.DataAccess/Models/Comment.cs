using System.ComponentModel.DataAnnotations;

namespace TicketingSystem.DataAccess.Models
{
    public class Comment
    {
        [Key]
        public Guid Id { get; set; }
        public string Message { get; set; }

        public DateTime CreateDate { get; set; }
        public Guid CreatedBy { get; set; } 
        public User CreatedByUser { get; set; }
        public Guid TicketId { get; set; } 
        public Ticket Ticket { get; set; }
    }
}
