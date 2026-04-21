using CourseCatalogService.Application.Interfaces;
using CourseCatalogService.Application.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseCatalogService.Presentation.Controllers
{
    [ApiController]
    [Route("upload")]
    [Authorize(Roles = "Admin,Instructor")]
    public class UploadController(IBlobStorageService blobStorageService, ILogger<UploadController> logger) : ControllerBase
    {
        private readonly IBlobStorageService _blob = blobStorageService;
        private readonly ILogger<UploadController> _logger = logger;

        [HttpPost("video")]
        [RequestSizeLimit(500_000_000)]
        public async Task<IActionResult> UploadVideo(IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file provided." });

            try
            {
                using var stream = file.OpenReadStream();
                var url = await _blob.UploadAsync(stream, file.FileName, "videos", file.ContentType, cancellationToken);

                return Ok(new UploadResponseDto
                {
                    Url = url,
                    FileName = file.FileName,
                    FileSizeBytes = file.Length,
                    ContentType = file.ContentType
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Video upload failed");
                return StatusCode(500, new { message = "Upload failed." });
            }
        }

        [HttpPost("document")]
        [RequestSizeLimit(50_000_000)]
        public async Task<IActionResult> UploadDocument(IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file provided." });

            try
            {
                using var stream = file.OpenReadStream();
                var url = await _blob.UploadAsync(stream, file.FileName, "documents", file.ContentType, cancellationToken);

                return Ok(new UploadResponseDto
                {
                    Url = url,
                    FileName = file.FileName,
                    FileSizeBytes = file.Length,
                    ContentType = file.ContentType
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Document upload failed");
                return StatusCode(500, new { message = "Upload failed." });
            }
        }
    }
}
