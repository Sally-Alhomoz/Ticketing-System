using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedDTOs;
using SharedDTOs.Enum;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
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

        [Authorize]
        [HttpPost]
        [SwaggerOperation(
            Summary = "Create new ticket.",
            Description = "Submits a new ticket to the system. Requires Authentication")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ticket added succssfully")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "An error occurred while saving the ticket")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required")]
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


        [Authorize]
        [HttpGet("GetTicketById")]
        [SwaggerOperation(
            Summary = "Get ticket details by ID.",
            Description = "Retrieves full details of a specific ticket using its ID. Requires Authentication")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ticket details retrieved successfully")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "No ticket found with the provided ID.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required")]
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

        [Authorize]
        [HttpPatch("AssignTo")]
        [SwaggerOperation(
            Summary = "Assign ticket to support",
            Description = "Assigns the specified ticket to the currently authenticated user. The ticket must be unassigned.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ticket assigned successfully")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ticket is already assigned to another user")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Ticket ID does not exist")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required")]
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

        [Authorize]
        [HttpPatch("SetPriority")]
        [SwaggerOperation(
            Summary = "Update ticket priority.",
           Description = "Changes the priority level (Low, Medium, High) of an existing ticket. Requires Authentication")]
        [SwaggerResponse(StatusCodes.Status200OK, "Priority updated successfully.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Ticket ID does not exist.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required.")]
        public async Task<IActionResult> SetTicketPriority(Guid ticketId, TicketPriority priority)
        {
            _logger.LogInformation("Patch called to set ticket priority");

            var result = await _ticketManager.SetPriprity(ticketId, priority);

            if(!result)
            {
                _logger.LogWarning("Ticket not found");
                return NotFound("Ticket not found.");
            }

            return Ok("Ticket priority updated seccessfully.");
        }

        [Authorize]
        [HttpPatch("UpdateStatus")]
        [SwaggerOperation(
            Summary = "Update ticket status.",
           Description = "Updates the workflow state of the ticket (e.g., Open, InProgress, Resolved). Requires Authentication")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ticket statua updated successfully")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Ticket ID does not exist")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required")]
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

        [Authorize]
        [HttpDelete]
        [SwaggerOperation(
            Summary = "Delete a ticket.",
           Description = "Removes a ticket from the system. Requires Authentication")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ticket statua updated successfully.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Ticket ID does not exist.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required.")]
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

        [Authorize]
        [HttpGet]
        [SwaggerOperation(
            Summary = "List all tickets (Paged).",
           Description = "Retrieves a paginated list of tickets with support for searching and sorting.")]
        [SwaggerResponse(StatusCodes.Status200OK, "List of tickets retrieved successfully.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required")]
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