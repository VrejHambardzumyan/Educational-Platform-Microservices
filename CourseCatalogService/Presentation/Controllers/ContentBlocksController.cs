using CourseCatalogService.Application.Interfaces;
using CourseCatalogService.Application.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseCatalogService.Presentation.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize(Roles = "Admin,Instructor")]
    public class ContentBlocksController(IContentBlockService contentBlockService, ILogger<ContentBlocksController> logger) : ControllerBase
    {
        private readonly IContentBlockService _service = contentBlockService;
        private readonly ILogger<ContentBlocksController> _logger = logger;

        [HttpGet("lessons/{lessonId}/blocks")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByLesson(int lessonId, CancellationToken cancellationToken)
        {
            var result = await _service.GetByLessonIdAsync(lessonId, cancellationToken);
            return Ok(result);
        }

        [HttpPost("lessons/{lessonId}/blocks")]
        public async Task<IActionResult> Add(int lessonId, [FromBody] ContentBlockRequestDto dto, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _service.AddAsync(lessonId, dto, cancellationToken);
                return CreatedAtAction(nameof(Add), new { lessonId }, result);
            }
            catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add content block");
                return StatusCode(500, new { message = "Failed to add content block." });
            }
        }

        [HttpPut("blocks/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ContentBlockRequestDto dto, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _service.UpdateAsync(id, dto, cancellationToken);
                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpDelete("blocks/{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var deleted = await _service.DeleteAsync(id, cancellationToken);
            if (!deleted) return NotFound();
            return NoContent();
        }

        [HttpPut("lessons/{lessonId}/blocks/reorder")]
        public async Task<IActionResult> Reorder(int lessonId, [FromBody] ReorderRequestDto dto, CancellationToken cancellationToken)
        {
            await _service.ReorderAsync(lessonId, dto.BlockIds, cancellationToken);
            return NoContent();
        }
    }
}
