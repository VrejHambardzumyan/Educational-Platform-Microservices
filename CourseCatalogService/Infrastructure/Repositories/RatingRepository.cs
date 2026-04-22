using CourseCatalogService.Infrastructure.Entities;
using CourseCatalogService.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CourseCatalogService.Infrastructure.Repositories
{
    public class RatingRepository(CourseDbContext context) : IRatingRepository
    {
        public async Task<CourseRating?> GetByUserAndCourseAsync(int userId, int courseId) =>
            await context.CourseRatings
                .FirstOrDefaultAsync(r => r.UserId == userId && r.CourseId == courseId);

        public async Task<IEnumerable<CourseRating>> GetByCourseAsync(int courseId) =>
            await context.CourseRatings
                .Where(r => r.CourseId == courseId)
                .AsNoTracking()
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

        public async Task AddAsync(CourseRating rating)
        {
            context.CourseRatings.Add(rating);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CourseRating rating)
        {
            rating.UpdatedAt = DateTime.UtcNow;
            context.CourseRatings.Update(rating);
            await context.SaveChangesAsync();
        }

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
