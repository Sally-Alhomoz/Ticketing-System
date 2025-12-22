using System.ComponentModel.DataAnnotations;
using SharedDTOs.Enum;

namespace TicketingSystem.DataAccess.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public Role Role { get; set; }
        public bool IsActive { get; set; }
    }
}
