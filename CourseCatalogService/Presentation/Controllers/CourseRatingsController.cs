using CourseCatalogService.Application.Interfaces;
using CourseCatalogService.Application.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseCatalogService.Presentation.Controllers
{
    [ApiController]
    [Route("courses/{courseId}/ratings")]
    public class CourseRatingsController(IRatingService ratingService) : ControllerBase
    {
        [HttpPost]
        [Authorize(Roles = "Admin,Instructor,Student")]
        public async Task<IActionResult> UpsertRating(int courseId, [FromBody] RatingRequestDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId is null) return Unauthorized();

            var result = await ratingService.UpsertRatingAsync(courseId, userId.Value, dto);
            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Instructor,Student")]
        public async Task<IActionResult> GetRatings(int courseId)
        {
            var summary = await ratingService.GetCourseRatingsAsync(courseId);
            if (summary is null) return NotFound(new { message = $"Course {courseId} not found." });
            return Ok(summary);
        }

        [HttpGet("my")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMyRating(int courseId)
        {
            var userId = GetCurrentUserId();
            if (userId is null) return Unauthorized();

            var rating = await ratingService.GetMyRatingAsync(courseId, userId.Value);
            if (rating is null) return NotFound(new { message = "You have not rated this course yet." });
            return Ok(rating);
        }

        private int? GetCurrentUserId()
        {
            var claim = User.FindFirst("userId");
            if (claim != null && int.TryParse(claim.Value, out var id))
                return id;
            return null;
        }
    }
}
