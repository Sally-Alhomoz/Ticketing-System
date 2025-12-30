using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedDTOs;
using SharedDTOs.Enum;
using System.Security.Claims;
using TicketingSystem.DataAccess.Models;
using TicketingSystem.Services.Interfaces;

namespace TicketingSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketManager;
        private readonly ILogger<TicketController> _logger;

        public TicketController(ITicketService ticketManager, ILogger<TicketController> logger)
        {
            _ticketManager = ticketManager;
            _logger = logger;
        }

        private Guid CurrentUserId
        {
            get
            {
                var claimValue = User.FindFirstValue(ClaimTypes.NameIdentifier)
                                 ?? User.FindFirstValue("sub")
                                 ?? throw new UnauthorizedAccessException("User ID not found in token.");

                return Guid.Parse(claimValue);
            }
        }

        /// <summary>
        /// Create new ticket.
        /// </summary>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateTicketDto ticket)
        {
            _logger.LogInformation("Post called tp add a ticket.");

            try
            {
                await _ticketManager.Add(ticket, CurrentUserId);
                _logger.LogInformation("Ticket added succssfully.");
                return Ok("Ticket added successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding a ticket.");
                return StatusCode(500, "An internal error occurred while creating the ticket.");
            }
        }

        /// <summary>
        /// Get Ticket by its Id
        /// </summary>
        [Authorize]
        [HttpGet("GetTicketById")]
        public async Task<IActionResult> GetTicketById(Guid ticketId)
        {
            _logger.LogInformation("Get Ticket by its Id.");

            var ticket = await _ticketManager.GetTicketById(ticketId);

            if (ticket == null)
            {
                _logger.LogWarning("Ticket not found");
                return NotFound("Ticket not found.");
            }

            return Ok(ticket);
        }

        /// <summary>
        /// Assign ticket to user.
        /// </summary>
        [Authorize]
        [HttpPatch("AssignTo")]
        public async Task<IActionResult> AssignTicketToUser(Guid ticketId)
        {
            _logger.LogInformation("Patch called to assign ticket to support staff.");

            var exist = await _ticketManager.GetTicketById(ticketId);

            if (exist == null)
            {
                _logger.LogWarning("Ticket not found");
                return NotFound("Ticket not found.");
            }

            var result = await _ticketManager.AssignTicket(ticketId, CurrentUserId);

            if (!result && exist.AssignedTo != null)
            {
                return BadRequest("Cannot assign ticket, ticket already assigned !");
            }

            return Ok("Ticket assigned successfully.");
        }

        /// <summary>
        /// Set ticket priority.
        /// </summary>
        [Authorize]
        [HttpPatch("SetPriority")]
        public async Task<IActionResult> SetTicketPriority(Guid ticketId, TicketPriority priority)
        {
            _logger.LogInformation("Patch called to set ticket priority");

            var result = await _ticketManager.SetPriprity(ticketId, priority);

            if(!result)
            {
                _logger.LogWarning("Ticket not found");
                return NotFound("Ticket not found.");
            }

            return Ok("Ticket updated seccessfully.");
        }

        /// <summary>
        /// Update ticket status.
        /// </summary>
        [Authorize]
        [HttpPatch("UpdateStatus")]
        public async Task<IActionResult> UpdateTicketStatus(Guid ticketId, TicketStatus newStatus)
        {
            _logger.LogInformation("Patch called to update ticket status.");

            var result = await _ticketManager.UpdateTicketStatus(ticketId, newStatus, CurrentUserId);

            if (!result)
            {
                _logger.LogWarning("Ticket not found");
                return NotFound("Ticket not found.");
            }

            return Ok("Ticket statua updated successfully.");
        }

        /// <summary>
        /// Delete ticket.
        /// </summary>
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> Delete(Guid ticketId)
        {
            _logger.LogInformation("Delete called to delete a ticket.");

            var result = await _ticketManager.DeleteTicket(ticketId, CurrentUserId);

            if (!result)
            {
                _logger.LogWarning("Ticket not found");
                return NotFound("Ticket not found.");
            }

            return Ok("Ticket deleted successfully.");
        }

        /// <summary>
        /// Get all tickets.
        /// </summary>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Read(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string search = "",
            [FromQuery] string sortBy = "title",
            [FromQuery] string sortDirection = "asc")
        {
            var (ticktes, totalCount) = await _ticketManager.GetTicketsPaged(page, pageSize, search, sortBy, sortDirection);

            return Ok(new
            {
                items = ticktes,
                totalCount = totalCount
            });
        }
    }
}
