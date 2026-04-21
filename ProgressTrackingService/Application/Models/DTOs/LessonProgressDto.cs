namespace ProgressTrackingService.Application.Models.DTOs
{
    public class LessonProgressDto
    {
        public int LessonId { get; init; }
        public bool IsCompleted { get; init; }
        public DateTime? CompletedAt { get; init; }
    }
}
