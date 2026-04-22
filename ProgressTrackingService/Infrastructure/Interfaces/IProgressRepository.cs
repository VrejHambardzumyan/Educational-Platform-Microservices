using ProgressTrackingService.Infrastructure.Entities;

namespace ProgressTrackingService.Infrastructure.Interfaces
{
    public interface IProgressRepository
    {
        Task<LessonProgress?> GetLessonProgressAsync(int userId, int lessonId, CancellationToken ct = default);
        Task<IEnumerable<LessonProgress>> GetCourseProgressDetailsAsync(int userId, int courseId, CancellationToken ct = default);
        Task<CourseProgress?> GetCourseProgressAsync(int userId, int courseId, CancellationToken ct = default);
        Task<IEnumerable<CourseProgress>> GetAllCourseProgressAsync(int userId, CancellationToken ct = default);

        /// <summary>
        /// Persists both the lesson completion and the updated course aggregate in a single
        /// database transaction. Either both writes succeed or neither does.
        /// </summary>
        Task UpsertProgressAtomicAsync(
            LessonProgress lessonProgress,
            CourseProgress courseProgress,
            CancellationToken ct = default);
    }
}
