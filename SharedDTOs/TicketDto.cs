using SharedDTOs.Enum;

namespace SharedDTOs
{
    public class TicketDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public TicketStatus Status { get; set; }
        public TicketPriority Priority { get; set; }
        public DateTime CreateDate { get; set; }
        public int productId { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? AssignedTo { get; set; }
        public string CreatedByFullName { get; set; }
        public string AssignedToFullName { get; set; }
        public string ProductName { get; set; }

    }
}
