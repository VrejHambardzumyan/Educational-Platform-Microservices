namespace ProgressTrackingService.Application.Models.DTOs
{
    /// <summary>
    /// Payload sent by the frontend when a student finishes a lesson.
    /// TotalLessonsInCourse is intentionally absent — the service fetches the
    /// authoritative count from CourseCatalogService.
    /// </summary>
    public sealed class MarkLessonCompleteDto
    {
        public int CourseId { get; set; }
        public int LessonId { get; set; }
    }
}
