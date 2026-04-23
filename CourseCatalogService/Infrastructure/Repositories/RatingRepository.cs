using CourseCatalogService.Infrastructure.Entities;
using CourseCatalogService.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CourseCatalogService.Infrastructure.Repositories
{
    public class RatingRepository(CourseDbContext context) : IRatingRepository
    {
        public async Task<CourseRating> UpsertAsync(int userId, int courseId, int rating, string? feedback, CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;

            // Try to update the existing row first (one atomic SQL statement)
            var updated = await context.CourseRatings
                .Where(r => r.UserId == userId && r.CourseId == courseId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(r => r.Rating, rating)
                    .SetProperty(r => r.Feedback, feedback)
                    .SetProperty(r => r.UpdatedAt, now),
                    cancellationToken);

            if (updated > 0)
                return await context.CourseRatings
                    .AsNoTracking()
                    .FirstAsync(r => r.UserId == userId && r.CourseId == courseId, cancellationToken);

            // Nothing existed — insert it
            var entity = new CourseRating
            {
                CourseId = courseId,
                UserId = userId,
                Rating = rating,
                Feedback = feedback
            };
            context.CourseRatings.Add(entity);
            try
            {
                await context.SaveChangesAsync(cancellationToken);
                return entity;
            }
            catch (DbUpdateException)
            {
                // Concurrent insert raced us — update the row they just inserted
                context.ChangeTracker.Clear();
                await context.CourseRatings
                    .Where(r => r.UserId == userId && r.CourseId == courseId)
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(r => r.Rating, rating)
                        .SetProperty(r => r.Feedback, feedback)
                        .SetProperty(r => r.UpdatedAt, now),
                        cancellationToken);
                return await context.CourseRatings
                    .AsNoTracking()
                    .FirstAsync(r => r.UserId == userId && r.CourseId == courseId, cancellationToken);
            }
        }

        public async Task<CourseRating?> GetByUserAndCourseAsync(int userId, int courseId) =>
            await context.CourseRatings
                .FirstOrDefaultAsync(r => r.UserId == userId && r.CourseId == courseId);

        public async Task<IEnumerable<CourseRating>> GetByCourseAsync(int courseId) =>
            await context.CourseRatings
                .Where(r => r.CourseId == courseId)
                .AsNoTracking()
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

        public async Task<(double Average, int Count)> RecalculateAsync(int courseId)
        {
            var ratings = await context.CourseRatings
                .Where(r => r.CourseId == courseId)
                .Select(r => r.Rating)
                .ToListAsync();

            if (ratings.Count == 0) return (0, 0);
            return (Math.Round(ratings.Average(), 1), ratings.Count);
        }
    }
}
