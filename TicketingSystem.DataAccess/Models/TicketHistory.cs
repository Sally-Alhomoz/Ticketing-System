using System.ComponentModel.DataAnnotations;
using SharedDTOs.Enum;

namespace TicketingSystem.DataAccess.Models
{
    public class TicketHistory
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime ChangeDate { get; set; }
        public TicketStatus NewStatus { get; set; }
        public Guid TicketId { get; set; }
        public Ticket Ticket { get; set; }
        public Guid ChangedBy { get; set; }
        public User ChangedByUser { get; set; }
    }
}
