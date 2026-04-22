namespace ProgressTrackingService.Application.Models.DTOs
{
    /// <summary>Minimal projection of a CourseCatalog lesson — only the fields needed for validation.</summary>
    public sealed class CatalogLessonDto
    {
        public int Id { get; init; }
        public int CourseId { get; init; }
    }
}
