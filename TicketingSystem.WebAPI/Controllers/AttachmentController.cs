using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TicketingSystem.Services.Interfaces;

namespace TicketingSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttachmentController : ControllerBase
    {
        private readonly IAttachmentService _attachmentManager;
        private readonly ILogger<AttachmentController> _logger;

        public AttachmentController(IAttachmentService attachmentManager, ILogger<AttachmentController> logger)
        {
            _attachmentManager = attachmentManager;
            _logger = logger;
        }

        private string GetContentType(string fileName)
        {
            var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(fileName, out var contentType))
            {
                contentType = "application/octet-stream"; 
            }
            return contentType;
        }


        [Authorize]
        [HttpGet("Download/{id}")]
        [SwaggerOperation(
            Summary = "Download a file.",
            Description = "Downloads a file by its ID. Requires Authentication")]
        [SwaggerResponse(StatusCodes.Status200OK, "Attachment downloaded successfully")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "No Attachmentfound with the provided ID.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required")]
        public async Task<IActionResult> Download(Guid id)
        {
            _logger.LogInformation("Attempting to download attachment {Id}", id);

            var attachment = await _attachmentManager.GetAttachmentById(id);

            if (attachment == null)
            {
                _logger.LogWarning("Attachment {Id} not found in database", id);
                return NotFound("Attachment not found.");
            }

            var fileStream = new FileStream(attachment.FilePath, FileMode.Open, FileAccess.Read);

            var contentType = GetContentType(attachment.FileName);

            return File(fileStream, contentType);
        }
    }
}
