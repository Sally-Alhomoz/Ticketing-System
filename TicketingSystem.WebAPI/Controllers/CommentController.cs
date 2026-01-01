using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedDTOs;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using TicketingSystem.DataAccess.Models;
using TicketingSystem.Services.Interfaces;
using TicketingSystem.Services.Services;

namespace TicketingSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentManager;
        private readonly ILogger<CommentController> _logger;

        public CommentController(ICommentService commentManager, ILogger<CommentController> logger)
        {
            _commentManager = commentManager;
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
            Summary = "Add new comment.",
            Description = "Add new comment to ticket. Requires Authentication")]
        [SwaggerResponse(StatusCodes.Status200OK, "Comment added successfully")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error occurred")]
        public async Task<IActionResult> Create([FromForm] CreateCommentDto comment)
        {
            _logger.LogInformation("Post is called to create new comment.");

            try
            {
                await _commentManager.Add(comment, CurrentUserId);
                _logger.LogInformation("Comment added succssfully.");
                return Ok("Comment added successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding a comment.");
                return StatusCode(500, "An internal error occurred while creating the comment.");
            }
        }

        [Authorize]
        [HttpGet("ByTicketId")]
        [SwaggerOperation(
            Summary = "Retrieves all comments for a ticket.",
            Description = "Retrieves all comments of a specific ticket using its ID. Requires Authentication")]
        [SwaggerResponse(StatusCodes.Status200OK, "Comments retrieved successfully")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "No comment found with the provided ID.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required")]
        public async Task<IActionResult> GetCommentByTicketId(Guid ticketId)
        {
            _logger.LogInformation("Get is called to get comment by the ticket id.");

            var comments = await _commentManager.GetCommentsByTicketId(ticketId);

            if (comments == null)
            {
                _logger.LogWarning("No comment found for this ticket");
                return NotFound("No comment found for this ticket.");
            }

            return Ok(comments);
        }

        [Authorize]
        [HttpGet("ByUserId")]
        [SwaggerOperation(
            Summary = "Retrieves all comments for a user.",
            Description = "Retrieves all comments of a specific user using its ID. Requires Authentication")]
        [SwaggerResponse(StatusCodes.Status200OK, "Comments retrieved successfully")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "No comment found with the provided ID.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required")]
        public async Task<IActionResult> GetCommentByUserId(Guid userId)
        {
            _logger.LogInformation("Get is called to get comment by the user id.");

            var comments = await _commentManager.GetCommentsByUserId(userId);

            if (comments == null)
            {
                _logger.LogWarning("No comment found for this user.");
                return NotFound("No comment found for this user.");
            }

            return Ok(comments);
        }
    }
}
