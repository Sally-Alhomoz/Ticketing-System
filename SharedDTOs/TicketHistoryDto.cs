using SharedDTOs.Enum;

namespace SharedDTOs
{
    public class TicketHistoryDto
    {
        public Guid Id { get; set; }
        public DateTime ChangeDate { get; set; }
        public TicketStatus NewStatus { get; set; }
        public Guid TicketId { get; set; }
        public string TicketTitle { get; set; }
        public Guid ChangedBy { get; set; }
        public string ChangedyFullName { get; set; }
    }
}
