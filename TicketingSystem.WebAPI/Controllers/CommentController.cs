using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedDTOs;
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

        /// <summary>
        /// Create new comment.
        /// </summary>
        [Authorize]
        [HttpPost]
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

        /// <summary>
        /// Get comment by ticket Id.
        /// </summary>
        [Authorize]
        [HttpGet("ByTicketId")]
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

        /// <summary>
        /// Get comment by user Id.
        /// </summary>
        [Authorize]
        [HttpGet("ByUserId")]
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
