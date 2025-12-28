using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedDTOs;
using SharedDTOs.Enum;
using System.Security.Claims;
using TicketingSystem.Services.Interfaces;

namespace TicketingSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketHistoryController : ControllerBase
    {
        private readonly ITicketHistoryService _historyManager;
        private readonly ILogger<TicketHistoryController> _logger;

        public TicketHistoryController(ITicketHistoryService historyManager, ILogger<TicketHistoryController> logger)
        {
            _historyManager = historyManager;
            _logger = logger;
        }

        private Guid CurrentUserId
        {
            get
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                  ?? User.FindFirst("sub")?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    throw new UnauthorizedAccessException("Invalid or missing user ID in token.");
                }

                return userId;
            }
        }

        /// <summary>
        /// Create new ticket history record.
        /// </summary>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(Guid ticketId, TicketStatus newStatus)
        {
            _logger.LogInformation("Post is called to create new ticket history.");

            try
            {
                await _historyManager.AddRecord(ticketId, newStatus, CurrentUserId);
                _logger.LogInformation("Ticket record added succssfully.");
                return Ok("Ticket record added successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding a ticket history.");
                return StatusCode(500, "An internal error occurred while creating the ticket history.");
            }
        }

        /// <summary>
        /// Get hirtory for ticket.
        /// </summary>
        [Authorize]
        [HttpGet("GetByTicketId")]
        public async Task<IActionResult> GetTicketHistoryById(Guid ticketId)
        {
            _logger.LogInformation("Get called to get hirtory for ticket by ticket Id");

            var records = await _historyManager.GetTicketHistoryById(ticketId);

            if (records == null)
            {
                _logger.LogWarning("No records found for this ticket.");
                return NotFound("No records found for this ticket.");
            }

            return Ok(records);
        }

        /// <summary>
        /// Get recent hstory for ticket.
        /// </summary>
        [Authorize]
        [HttpGet("GetRecent")]
        public async Task<IActionResult> GetLatestForTicket(Guid ticketId)
        {
            _logger.LogInformation("Get called to get recent hirtory for ticket by ticket Id");

            var record = await _historyManager.GetLatestForTicket(ticketId);

            if (record == null)
            {
                _logger.LogWarning("No records found for this ticket.");
                return NotFound("No records found for this ticket.");
            }

            return Ok(record);
        }
    }
}
