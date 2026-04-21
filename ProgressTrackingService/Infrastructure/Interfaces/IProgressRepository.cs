using ProgressTrackingService.Infrastructure.Entities;

namespace ProgressTrackingService.Infrastructure.Interfaces
{
    public interface IProgressRepository
    {
        Task<LessonProgress?> GetLessonProgressAsync(int userId, int lessonId, CancellationToken ct = default);
        Task<IEnumerable<LessonProgress>> GetCourseProgressDetailsAsync(int userId, int courseId, CancellationToken ct = default);
        Task<CourseProgress?> GetCourseProgressAsync(int userId, int courseId, CancellationToken ct = default);
        Task<IEnumerable<CourseProgress>> GetAllCourseProgressAsync(int userId, CancellationToken ct = default);
        Task UpsertLessonProgressAsync(LessonProgress progress);
        Task UpsertCourseProgressAsync(CourseProgress progress);
    }
}
