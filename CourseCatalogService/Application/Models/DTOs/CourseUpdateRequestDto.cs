namespace CourseCatalogService.Application.Models.DTOs
{
    public class CourseUpdateRequestDto
    {
        public string? Title { get; init; }
        public string? Description { get; init; }
        public int? DurationInMonth { get; init; }
        public int? Price { get; init; }
        public string? Category { get; init; }
        public string? ThumbnailUrl { get; init; }
    }
}
