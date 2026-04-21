using ProgressTrackingService.Application.Models.DTOs;

namespace ProgressTrackingService.Application.Interfaces
{
    public interface IProgressService
    {
        Task MarkLessonCompleteAsync(int userId, MarkLessonCompleteDto dto, CancellationToken ct = default);
        Task<IEnumerable<LessonProgressDto>> GetCourseLessonProgressAsync(int userId, int courseId, CancellationToken ct = default);
        Task<CourseProgressDto?> GetCourseProgressAsync(int userId, int courseId, CancellationToken ct = default);
        Task<IEnumerable<CourseProgressDto>> GetAllProgressAsync(int userId, CancellationToken ct = default);
    }
}
