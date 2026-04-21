namespace ProgressTrackingService.Infrastructure.Entities
{
    public class CourseProgress
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CourseId { get; set; }
        public int TotalLessons { get; set; }
        public int CompletedLessons { get; set; }
        public int ProgressPercentage => TotalLessons == 0 ? 0 : (int)Math.Round((double)CompletedLessons / TotalLessons * 100);
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
