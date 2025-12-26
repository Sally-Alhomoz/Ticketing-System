using System.ComponentModel.DataAnnotations;
using SharedDTOs.Enum;

namespace TicketingSystem.DataAccess.Models
{
    public class Ticket
    {
        [Key]
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public TicketStatus Status { get; set; }
        public TicketPriority Priority { get; set; }
        public DateTime CreateDate { get; set; }
        public int productId { get; set; }
        public Product product { get; set; }
        public Guid CreatedBy { get; set; }
        public virtual User Creator { get; set; }
        public Guid? AssignedTo { get; set; }
        public virtual User AssignedUser { get; set; }
    }
}
