using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        [ForeignKey("TicketId")]
        public Ticket Ticket { get; set; }
        public Guid ChangedBy { get; set; }
        [ForeignKey("ChangedBy")]
        public User ChangedByUser { get; set; }
    }
}
