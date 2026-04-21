using CourseCatalogService.Infrastructure.Entities;

namespace CourseCatalogService.Application.Models.DTOs
{
    public class ContentBlockResponseDto
    {
        public int Id { get; init; }
        public int LessonId { get; init; }
        public ContentBlockType Type { get; init; }
        public int Order { get; init; }
        public string? TextContent { get; init; }
        public string? VideoUrl { get; init; }
        public string? FileUrl { get; init; }
        public string? Metadata { get; init; }
    }
}
