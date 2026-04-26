namespace CourseCatalogService.Application.Models.DTOs
{
    public class LessonRequestDto
    {
        public required string Title { get; init; }
        public string? Description { get; init; }
        public int OrderIndex { get; init; }
        public int DurationInMinutes { get; init; }
    }
}
