namespace CourseCatalogService.Application.Models.DTOs
{
    public class CourseRatingSummaryDto
    {
        public double AverageRating { get; init; }
        public int RatingCount { get; init; }
        public IEnumerable<RatingResponseDto> Ratings { get; init; } = [];
    }
}
