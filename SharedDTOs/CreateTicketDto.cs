namespace SharedDTOs
{
    public class CreateTicketDto
    {
        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public int productId { get; set; }
    }
}
