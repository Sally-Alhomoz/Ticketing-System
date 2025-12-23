using SharedDTOs.Enum;

namespace SharedDTOs
{
    public class TicketDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public TicketStatus Status { get; set; }
        public TicketPriority Priority { get; set; }
        public DateTime CteateDate { get; set; }
        public int productId { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? AssignedTo { get; set; }
    }
}
