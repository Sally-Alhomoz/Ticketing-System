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
        public DateTime CteateDate { get; set; }
        public int productId { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? AssignedTo { get; set; }
        public bool IsDeleted { get; set; }
    }
}
