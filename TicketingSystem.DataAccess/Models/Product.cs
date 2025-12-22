using System.ComponentModel.DataAnnotations;

namespace TicketingSystem.DataAccess.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string ProductName { get; set; }
        public bool IsDeleted { get; set; }
    }
}
