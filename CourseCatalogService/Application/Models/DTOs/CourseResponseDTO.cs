namespace CourseCatalogService.Application.Models.DTOs
{
    public class CourseResponseDto
    {
        public int Id { get; init; }
        public required string Title { get; init; }
        public string? Description { get; init; }
        public required int DurationInMonth { get; init; }
        public required int Price { get; init; }
        public int InstructorId { get; init; }
        public string? Status { get; init; }
        public string? Category { get; init; }
        public string? ThumbnailUrl { get; init; }
        public double AverageRating { get; init; }
        public int RatingCount { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}
