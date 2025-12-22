using Microsoft.EntityFrameworkCore;
using TicketingSystem.DataAccess.Models;

namespace TicketingSystem.DataAccess
{
    public class TicketingSystemDBContext : DbContext
    {
        public TicketingSystemDBContext(DbContextOptions<TicketingSystemDBContext> options) : base(options)
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketHistory> TicketsHistory { get; set; }

        public DbSet<Comment> Comments { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Product> Products { get; set; }

    }
}
