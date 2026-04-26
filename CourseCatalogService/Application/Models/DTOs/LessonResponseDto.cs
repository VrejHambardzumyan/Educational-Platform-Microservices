namespace CourseCatalogService.Application.Models.DTOs
{
    public class LessonResponseDto
    {
        public int Id { get; init; }
        public int CourseId { get; init; }
        public int? SectionId { get; init; }
        public required string Title { get; init; }
        public string? Description { get; init; }
        public int OrderIndex { get; init; }
        public int DurationInMinutes { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}
