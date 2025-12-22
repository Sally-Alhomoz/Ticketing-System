using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketingSystem.DataAccess.Interfaces;
using TicketingSystem.DataAccess.Repositories;

namespace TicketingSystem.DataAccess.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TicketingSystemDBContext _db;
        private readonly ILoggerFactory _logger;

        public IUserRepository Users { get; private set; }
        public ITicketRepository Tickets { get; private set; }
        public IProductRepository Products { get; private set; }

        public UnitOfWork(TicketingSystemDBContext db, ILoggerFactory logger)
        {
            _db = db;
            _logger = logger;

            var userLogger = _logger.CreateLogger<UserRepository>();
            var ticketLogger = _logger.CreateLogger<TicketRepository>();
            var productLogger = _logger.CreateLogger<ProductRepository>();

            Users = new UserRepository(_db, userLogger);
            Tickets = new TicketRepository(_db, ticketLogger);
            Products = new ProductRepository(_db, productLogger);

        }

        public int Complete()
        {
            return _db.SaveChanges();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
