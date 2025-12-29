using SharedDTOs.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [ForeignKey("productId")]
        public Product product { get; set; }
        public Guid CreatedBy { get; set; }
        [ForeignKey("CreatedBy")]
        public virtual User Creator { get; set; }
        public Guid? AssignedTo { get; set; }
        [ForeignKey("AssignedTo")]
        public virtual User AssignedUser { get; set; }
    }
}
