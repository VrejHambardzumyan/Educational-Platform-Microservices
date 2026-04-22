using ProgressTrackingService.Application.HttpClients;
using ProgressTrackingService.Application.Interfaces;
using ProgressTrackingService.Application.Models.DTOs;
using ProgressTrackingService.Infrastructure.Entities;
using ProgressTrackingService.Infrastructure.Interfaces;

namespace ProgressTrackingService.Application.Services
{
    public class ProgressService(
        IProgressRepository progressRepo,
        IEnrollmentClient enrollmentClient,
        ICatalogClient catalogClient) : IProgressService
    {
        private readonly IProgressRepository _progressRepo = progressRepo;
        private readonly IEnrollmentClient   _enrollmentClient = enrollmentClient;
        private readonly ICatalogClient      _catalogClient = catalogClient;

        /// <summary>
        /// Marks a lesson as complete for the given user after performing three guards:
        /// <list type="number">
        ///   <item>User must have an active enrollment for the course.</item>
        ///   <item>The lesson must actually belong to that course (cross-service validation).</item>
        ///   <item>Total lesson count comes from CourseCatalog, never the client.</item>
        /// </list>
        /// Both the lesson record and the course aggregate are written in a single transaction.
        /// </summary>
        public async Task MarkLessonCompleteAsync(int userId, MarkLessonCompleteDto dto, CancellationToken ct = default)
        {
            // 1. Enrollment check — user must be an active paying member of this course
            var enrolled = await _enrollmentClient.IsEnrolledAsync(dto.CourseId, ct);
            if (!enrolled)
                throw new UnauthorizedAccessException(
                    $"User {userId} does not have an active enrollment for course {dto.CourseId}.");

            // 2. Fetch authoritative lesson list from CourseCatalog
            var lessons = await _catalogClient.GetCourseLessonsAsync(dto.CourseId, ct);
            if (lessons.Count == 0)
                throw new KeyNotFoundException(
                    $"Course {dto.CourseId} was not found or contains no lessons.");

            // 3. Cross-service lesson validation — lessonId must belong to this course
            if (!lessons.Any(l => l.Id == dto.LessonId))
                throw new KeyNotFoundException(
                    $"Lesson {dto.LessonId} does not belong to course {dto.CourseId}.");

            var totalLessons = lessons.Count;

            // 4. Calculate how many lessons will be completed after this operation.
            //    Using a set so re-completing an already-done lesson doesn't inflate the count.
            var existingCompleted = (await _progressRepo.GetCourseProgressDetailsAsync(userId, dto.CourseId, ct))
                .Where(l => l.IsCompleted)
                .Select(l => l.LessonId)
                .ToHashSet();

            existingCompleted.Add(dto.LessonId); // idempotent: already-completed lessons stay in the set
            var completedCount = existingCompleted.Count;

            var isNowComplete = completedCount >= totalLessons;

            // 5. Build entities
            var lessonProgress = new LessonProgress
            {
                UserId      = userId,
                CourseId    = dto.CourseId,
                LessonId    = dto.LessonId,
                IsCompleted = true,
                CompletedAt = DateTime.UtcNow
            };

            var courseProgress = new CourseProgress
            {
                UserId           = userId,
                CourseId         = dto.CourseId,
                TotalLessons     = totalLessons,
                CompletedLessons = completedCount,
                IsCompleted      = isNowComplete,
                CompletedAt      = isNowComplete ? DateTime.UtcNow : null
            };

            // 6. Persist both in one atomic transaction
            await _progressRepo.UpsertProgressAtomicAsync(lessonProgress, courseProgress, ct);
        }

        /// <summary>Returns the per-lesson completion state for every lesson the user has interacted with in a course.</summary>
        public async Task<IEnumerable<LessonProgressDto>> GetCourseLessonProgressAsync(
            int userId, int courseId, CancellationToken ct = default)
        {
            var lessons = await _progressRepo.GetCourseProgressDetailsAsync(userId, courseId, ct);
            return lessons.Select(l => new LessonProgressDto
            {
                LessonId    = l.LessonId,
                IsCompleted = l.IsCompleted,
                CompletedAt = l.CompletedAt
            });
        }

        /// <summary>Returns the aggregated progress summary for one course, or null when no progress exists yet.</summary>
        public async Task<CourseProgressDto?> GetCourseProgressAsync(
            int userId, int courseId, CancellationToken ct = default)
        {
            var cp = await _progressRepo.GetCourseProgressAsync(userId, courseId, ct);
            return cp is null ? null : MapToDto(cp);
        }

        /// <summary>Returns the aggregated progress summary for every course the user has started.</summary>
        public async Task<IEnumerable<CourseProgressDto>> GetAllProgressAsync(
            int userId, CancellationToken ct = default)
        {
            var all = await _progressRepo.GetAllCourseProgressAsync(userId, ct);
            return all.Select(MapToDto);
        }

        private static CourseProgressDto MapToDto(CourseProgress cp) => new()
        {
            CourseId          = cp.CourseId,
            TotalLessons      = cp.TotalLessons,
            CompletedLessons  = cp.CompletedLessons,
            ProgressPercentage = cp.ProgressPercentage,
            IsCompleted       = cp.IsCompleted,
            CompletedAt       = cp.CompletedAt
        };
    }
}
