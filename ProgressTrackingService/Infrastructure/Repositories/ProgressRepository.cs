using Microsoft.EntityFrameworkCore;
using ProgressTrackingService.Infrastructure.Entities;
using ProgressTrackingService.Infrastructure.Interfaces;

namespace ProgressTrackingService.Infrastructure.Repositories
{
    public class ProgressRepository(ProgressDbContext context) : IProgressRepository
    {
        private readonly ProgressDbContext _context = context;

        public async Task<LessonProgress?> GetLessonProgressAsync(int userId, int lessonId, CancellationToken ct = default)
        {
            return await _context.LessonProgress
                .FirstOrDefaultAsync(lp => lp.UserId == userId && lp.LessonId == lessonId, ct);
        }

        public async Task<IEnumerable<LessonProgress>> GetCourseProgressDetailsAsync(int userId, int courseId, CancellationToken ct = default)
        {
            return await _context.LessonProgress
                .Where(lp => lp.UserId == userId && lp.CourseId == courseId)
                .AsNoTracking()
                .ToListAsync(ct);
        }

        public async Task<CourseProgress?> GetCourseProgressAsync(int userId, int courseId, CancellationToken ct = default)
        {
            return await _context.CourseProgress
                .FirstOrDefaultAsync(cp => cp.UserId == userId && cp.CourseId == courseId, ct);
        }

        public async Task<IEnumerable<CourseProgress>> GetAllCourseProgressAsync(int userId, CancellationToken ct = default)
        {
            return await _context.CourseProgress
                .Where(cp => cp.UserId == userId)
                .AsNoTracking()
                .ToListAsync(ct);
        }

        public async Task UpsertLessonProgressAsync(LessonProgress progress)
        {
            var existing = await _context.LessonProgress
                .FirstOrDefaultAsync(lp => lp.UserId == progress.UserId && lp.LessonId == progress.LessonId);

            if (existing == null)
            {
                _context.LessonProgress.Add(progress);
            }
            else
            {
                existing.IsCompleted = progress.IsCompleted;
                existing.CompletedAt = progress.CompletedAt;
                existing.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpsertCourseProgressAsync(CourseProgress progress)
        {
            var existing = await _context.CourseProgress
                .FirstOrDefaultAsync(cp => cp.UserId == progress.UserId && cp.CourseId == progress.CourseId);

            if (existing == null)
            {
                _context.CourseProgress.Add(progress);
            }
            else
            {
                existing.CompletedLessons = progress.CompletedLessons;
                existing.TotalLessons = progress.TotalLessons;
                existing.IsCompleted = progress.IsCompleted;
                existing.CompletedAt = progress.CompletedAt;
                existing.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }
    }
}
