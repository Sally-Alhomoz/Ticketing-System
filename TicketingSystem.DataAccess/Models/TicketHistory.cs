using System.ComponentModel.DataAnnotations;
using SharedDTOs.Enum;

namespace TicketingSystem.DataAccess.Models
{
    public class TicketHistory
    {
        [Key]
        public Guid Id { get; set; }
        public DateOnly ChangeDate { get; set; }
        public TicketStatus PreviousStatus { get; set; }
        public TicketStatus NewStatus { get; set; }
        public Guid TicketId { get; set; }
        public Guid ChangedBy { get; set; }
    }
}
