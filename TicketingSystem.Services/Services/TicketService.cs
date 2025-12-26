using Microsoft.Extensions.Logging;
using SharedDTOs;
using SharedDTOs.Enum;
using System.Net.Sockets;
using TicketingSystem.DataAccess.Interfaces;
using TicketingSystem.DataAccess.Models;
using TicketingSystem.DataAccess.UnitOfWork;
using TicketingSystem.Services.Interfaces;

namespace TicketingSystem.Services.Services
{
    public class TicketService : ITicketService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<TicketService> _logger;

        public TicketService(IUnitOfWork uow ,ILogger<TicketService> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task Add(TicketDto t)
        {
            _logger.LogInformation("Adding a ticket");

            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                Title = t.Title,
                Description = t.Description,
                Status = TicketStatus.New,
                CreateDate=DateTime.UtcNow,
                Priority = t.Priority,
                productId = t.productId,
                CreatedBy = t.CreatedBy,
                AssignedTo = null
            };

            _uow.Tickets.Add(ticket);
            _logger.LogInformation("Ticket added to the repository");
            await _uow.Complete();
        }

        public async Task<bool> AssignTicket(Guid ticketId, Guid userid)
        {
            _logger.LogInformation("Assign ticket to support staff.");
            var ticket = await _uow.Tickets.GetTicketById(ticketId);

            if(ticket == null)
            {
                _logger.LogWarning("Ticket not found.");
                return false;
            }

            if (ticket.AssignedTo != null)
            {
                _logger.LogWarning("Ticket is already assigned.");
                return false;
            }

            ticket.AssignedTo = userid;
            ticket.Status = TicketStatus.InProgress;

            _uow.TicketsHistory.Add(new TicketHistory
            {
                TicketId = ticket.Id,
                ChangeDate = DateTime.Now,
                NewStatus = ticket.Status,
                ChangedBy = userid
            });

            _uow.Tickets.UpdateTicket(ticket);
            _logger.LogInformation("Ticket assigned successfully");
            await _uow.Complete();
            return true;
        }

        public async Task<bool> UpdateTicketStatus(Guid ticketId , TicketStatus newStatus, Guid userId)
        {
            _logger.LogInformation("Update ticket status");

            var ticket = await _uow.Tickets.GetTicketById(ticketId);

            if (ticket == null)
            {
                _logger.LogWarning("Ticket not found.");
                return false;
            }

            if(ticket.Status == newStatus)
            {
                return true;
            }

            var oldStatus = ticket.Status;
            ticket.Status = newStatus;

            _uow.TicketsHistory.Add(new TicketHistory
            {
                TicketId = ticket.Id,
                ChangeDate = DateTime.Now,
                NewStatus = newStatus,
                ChangedBy = userId
            });

            _uow.Tickets.UpdateTicket(ticket);
            await _uow.Complete();
            _logger.LogInformation("Ticket status updated from {old} to {new} successfully.",oldStatus,newStatus);
            return true;
        }

        public async Task<bool> SetPriprity(Guid ticketId, TicketPriority priority)
        {
            _logger.LogInformation("Set ticket priority.");

            var ticket = await _uow.Tickets.GetTicketById(ticketId);

            if (ticket == null)
            {
                _logger.LogWarning("Ticket not found.");
                return false;
            }

            ticket.Priority = priority;

            _logger.LogInformation("Ticket priority has been set successfully.");
            _uow.Tickets.UpdateTicket(ticket);
            await _uow.Complete();
            return true;
        }

        public async Task<bool> DeleteTicket(Guid ticketId, Guid userId)
        {
            _logger.LogInformation("Deleting a ticket.");

            var result = await UpdateTicketStatus(ticketId, TicketStatus.Deleted, userId);

            if (!result)
            {
                _logger.LogWarning("ticket not found.");
                return false;
            }

            await _uow.Complete();
            _logger.LogInformation("ticket with id : {id} deleted successfully.", ticketId);
            return true;
        }

        public async Task<TicketDto?> GetTicketById(Guid ticketId)
        {
            _logger.LogInformation("Retrievung ticket by Id");

            var ticket = await _uow.Tickets.GetTicketById(ticketId);

            if(ticket==null)
            {
                _logger.LogWarning("Ticket not found");
                return null;
            }

            var dto = new TicketDto
            {
                Id = ticketId,
                Title = ticket.Title,
                Description = ticket.Description,
                Priority = ticket.Priority,
                Status = ticket.Status,
                productId = ticket.productId,
                ProductName = ticket.product.ProductName,
                CreatedBy = ticket.CreatedBy,
                CreatedByFullName = ticket.Creator.FirstName + " " + ticket.Creator.LastName,
                AssignedTo = ticket.AssignedTo,
                AssignedToFullName = ticket.AssignedTo != null? ticket.AssignedUser.FirstName + " " + ticket.AssignedUser.LastName: "Unassigned",
                CreateDate = ticket.CreateDate
            };

            _logger.LogInformation("Ticket retrieved successfully");
            return dto;
        }

        public async Task<(List<TicketDto> ticktes, int totalCount)> GetTicketsPaged(
            int page = 1,
            int pageSize = 10,
            string search = "",
            string sortBy = "title",
            string sortDirection = "asc")
        {
            _logger.LogInformation("Retrieving paged tickets. Page: {page}, Size: {pageSize}", page, pageSize);

            var query = _uow.Tickets.GetTicktes();

            // GLOBAL SEARCH — all fields
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                query = query.Where(t =>
                    t.Title.ToLower().Contains(s) ||
                    t.Description.ToLower().Contains(s)||
                    (t.Creator.FirstName + " " + t.Creator.LastName).ToLower().Contains(s) ||
                    (t.AssignedTo != null && (t.AssignedUser.FirstName.ToLower().Contains(s) || t.AssignedUser.LastName.ToLower().Contains(s)))||
                    t.product.ProductName.ToLower().Contains(s)
                );
            }

            // FULL SORTING — every column
            query = (sortBy?.ToLower(), sortDirection?.ToLower()) switch
            {
                ("priority", "desc") => query.OrderByDescending(t => t.Priority),
                ("priority", _) => query.OrderBy(t => t.Priority),
                ("status", "desc") => query.OrderByDescending(t => t.Status),
                ("status", _) => query.OrderBy(t => t.Status),
                ("date", "desc") => query.OrderByDescending(t => t.CreateDate),
                ("date", _) => query.OrderBy(t => t.CreateDate),
                ("creator", "desc") => query.OrderByDescending(t => t.Creator.FirstName + " " + t.Creator.LastName),
                ("creator", _) => query.OrderBy(t => t.Creator.FirstName + " " + t.Creator.LastName),
                ("assigneduser", "desc") => query.OrderByDescending(t => t.AssignedTo != null ? t.AssignedUser.FirstName + " " + t.AssignedUser.LastName : ""),
                ("assigneduser", _) => query.OrderBy(t => t.AssignedTo != null ? t.AssignedUser.FirstName + " " + t.AssignedUser.LastName : ""),
                ("productname", "desc") => query.OrderByDescending(t => t.product.ProductName),
                ("productname", _) => query.OrderBy(t => t.product.ProductName),
                ("title", "desc") => query.OrderByDescending(t => t.Title),
                _ => query.OrderBy(t => t.Title)
            };

            int totalCount = query.Count();

            var ticktes = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TicketDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    AssignedTo = t.AssignedTo,
                    CreatedBy = t.CreatedBy,
                    CreateDate = t.CreateDate,
                    Priority = t.Priority,
                    productId = t.productId,
                    Status = t.Status,
                    CreatedByFullName = t.Creator.FirstName + " " + t.Creator.LastName,
                    AssignedToFullName = t.AssignedTo != null
                    ? t.AssignedUser.FirstName + " " + t.AssignedUser.LastName : "Unassigned",
                    ProductName =t.product.ProductName
                })
                .ToList();

            return (ticktes, totalCount);
        }
    }
}
