namespace ProgressTrackingService.Application.Models.DTOs
{
    public class CourseProgressDto
    {
        public int CourseId { get; init; }
        public int TotalLessons { get; init; }
        public int CompletedLessons { get; init; }
        public int ProgressPercentage { get; init; }
        public bool IsCompleted { get; init; }
        public DateTime? CompletedAt { get; init; }
    }
}
