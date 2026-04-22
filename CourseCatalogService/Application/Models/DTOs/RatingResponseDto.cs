namespace CourseCatalogService.Application.Models.DTOs
{
    public class RatingResponseDto
    {
        public int Id { get; init; }
        public int CourseId { get; init; }
        public int UserId { get; init; }
        public int Rating { get; init; }
        public string? Feedback { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
    }
}
