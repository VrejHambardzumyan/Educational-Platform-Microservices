namespace ProgressTrackingService.Application.Models.DTOs
{
    public class MarkLessonCompleteDto
    {
        public int CourseId { get; set; }
        public int LessonId { get; set; }
        public int TotalLessonsInCourse { get; set; }
    }
}
