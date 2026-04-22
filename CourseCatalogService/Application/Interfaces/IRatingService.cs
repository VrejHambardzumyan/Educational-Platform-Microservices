using CourseCatalogService.Application.Models.DTOs;

namespace CourseCatalogService.Application.Interfaces
{
    public interface IRatingService
    {
        Task<RatingResponseDto> UpsertRatingAsync(int courseId, int userId, RatingRequestDto dto);
        Task<CourseRatingSummaryDto?> GetCourseRatingsAsync(int courseId);
        Task<RatingResponseDto?> GetMyRatingAsync(int courseId, int userId);
    }
}
