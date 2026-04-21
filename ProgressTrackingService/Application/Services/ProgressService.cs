using ProgressTrackingService.Application.Interfaces;
using ProgressTrackingService.Application.Models.DTOs;
using ProgressTrackingService.Infrastructure.Entities;
using ProgressTrackingService.Infrastructure.Interfaces;

namespace ProgressTrackingService.Application.Services
{
    public class ProgressService(IProgressRepository progressRepo) : IProgressService
    {
        private readonly IProgressRepository _progressRepo = progressRepo;

        public async Task MarkLessonCompleteAsync(int userId, MarkLessonCompleteDto dto, CancellationToken ct = default)
        {
            var lessonProgress = new LessonProgress
            {
                UserId = userId,
                CourseId = dto.CourseId,
                LessonId = dto.LessonId,
                IsCompleted = true,
                CompletedAt = DateTime.UtcNow
            };
            await _progressRepo.UpsertLessonProgressAsync(lessonProgress);

            var completedLessons = (await _progressRepo.GetCourseProgressDetailsAsync(userId, dto.CourseId, ct))
                .Count(l => l.IsCompleted);

            var courseProgress = new CourseProgress
            {
                UserId = userId,
                CourseId = dto.CourseId,
                TotalLessons = dto.TotalLessonsInCourse,
                CompletedLessons = completedLessons,
                IsCompleted = dto.TotalLessonsInCourse > 0 && completedLessons >= dto.TotalLessonsInCourse,
                CompletedAt = (dto.TotalLessonsInCourse > 0 && completedLessons >= dto.TotalLessonsInCourse)
                    ? DateTime.UtcNow
                    : null
            };
            await _progressRepo.UpsertCourseProgressAsync(courseProgress);
        }

        public async Task<IEnumerable<LessonProgressDto>> GetCourseLessonProgressAsync(int userId, int courseId, CancellationToken ct = default)
        {
            var lessons = await _progressRepo.GetCourseProgressDetailsAsync(userId, courseId, ct);
            return lessons.Select(l => new LessonProgressDto
            {
                LessonId = l.LessonId,
                IsCompleted = l.IsCompleted,
                CompletedAt = l.CompletedAt
            });
        }

        public async Task<CourseProgressDto?> GetCourseProgressAsync(int userId, int courseId, CancellationToken ct = default)
        {
            var cp = await _progressRepo.GetCourseProgressAsync(userId, courseId, ct);
            if (cp == null) return null;
            return MapToDto(cp);
        }

        public async Task<IEnumerable<CourseProgressDto>> GetAllProgressAsync(int userId, CancellationToken ct = default)
        {
            var all = await _progressRepo.GetAllCourseProgressAsync(userId, ct);
            return all.Select(MapToDto);
        }

        private static CourseProgressDto MapToDto(CourseProgress cp) => new()
        {
            CourseId = cp.CourseId,
            TotalLessons = cp.TotalLessons,
            CompletedLessons = cp.CompletedLessons,
            ProgressPercentage = cp.ProgressPercentage,
            IsCompleted = cp.IsCompleted,
            CompletedAt = cp.CompletedAt
        };
    }
}
