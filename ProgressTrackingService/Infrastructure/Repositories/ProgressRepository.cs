using Microsoft.EntityFrameworkCore;
using ProgressTrackingService.Infrastructure.Entities;
using ProgressTrackingService.Infrastructure.Interfaces;

namespace ProgressTrackingService.Infrastructure.Repositories
{
    public class ProgressRepository(ProgressDbContext context) : IProgressRepository
    {
        private readonly ProgressDbContext _context = context;

        public async Task<LessonProgress?> GetLessonProgressAsync(
            int userId, int lessonId, CancellationToken ct = default) =>
            await _context.LessonProgress
                .FirstOrDefaultAsync(lp => lp.UserId == userId && lp.LessonId == lessonId, ct);

        public async Task<IEnumerable<LessonProgress>> GetCourseProgressDetailsAsync(
            int userId, int courseId, CancellationToken ct = default) =>
            await _context.LessonProgress
                .Where(lp => lp.UserId == userId && lp.CourseId == courseId)
                .AsNoTracking()
                .ToListAsync(ct);

        public async Task<CourseProgress?> GetCourseProgressAsync(
            int userId, int courseId, CancellationToken ct = default) =>
            await _context.CourseProgress
                .FirstOrDefaultAsync(cp => cp.UserId == userId && cp.CourseId == courseId, ct);

        public async Task<IEnumerable<CourseProgress>> GetAllCourseProgressAsync(
            int userId, CancellationToken ct = default) =>
            await _context.CourseProgress
                .Where(cp => cp.UserId == userId)
                .AsNoTracking()
                .ToListAsync(ct);

        /// <summary>
        /// Stages both upserts and commits them in one transaction.
        /// Rolls back automatically if either write fails.
        /// </summary>
        public async Task UpsertProgressAtomicAsync(
            LessonProgress lessonProgress,
            CourseProgress courseProgress,
            CancellationToken ct = default)
        {
            await using var tx = await _context.Database.BeginTransactionAsync(ct);
            try
            {
                await StageLessonUpsertAsync(lessonProgress, ct);
                await StageCourseUpsertAsync(courseProgress, ct);

                await _context.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch
            {
                await tx.RollbackAsync(ct);
                throw;
            }
        }

        // Stages lesson upsert without calling SaveChanges — called inside the atomic method.
        private async Task StageLessonUpsertAsync(LessonProgress progress, CancellationToken ct)
        {
            var existing = await _context.LessonProgress
                .FirstOrDefaultAsync(
                    lp => lp.UserId == progress.UserId && lp.LessonId == progress.LessonId, ct);

            if (existing is null)
            {
                _context.LessonProgress.Add(progress);
            }
            else
            {
                existing.IsCompleted = progress.IsCompleted;
                existing.CompletedAt = progress.CompletedAt;
                existing.UpdatedAt  = DateTime.UtcNow;
            }
        }

        // Stages course progress upsert without calling SaveChanges — called inside the atomic method.
        private async Task StageCourseUpsertAsync(CourseProgress progress, CancellationToken ct)
        {
            var existing = await _context.CourseProgress
                .FirstOrDefaultAsync(
                    cp => cp.UserId == progress.UserId && cp.CourseId == progress.CourseId, ct);

            if (existing is null)
            {
                _context.CourseProgress.Add(progress);
            }
            else
            {
                existing.CompletedLessons = progress.CompletedLessons;
                existing.TotalLessons     = progress.TotalLessons;
                existing.IsCompleted      = progress.IsCompleted;
                existing.CompletedAt      = progress.CompletedAt;
                existing.UpdatedAt        = DateTime.UtcNow;
            }
        }
    }
}
