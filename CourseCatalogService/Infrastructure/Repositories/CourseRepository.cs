using CourseCatalogService.Infrastructure.Entities;
using CourseCatalogService.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CourseCatalogService.Infrastructure.Repositories
{
    public class CourseRepository(CourseDbContext dbContext) : ICourseRepository
    {
        private readonly CourseDbContext _context = dbContext;

        public async Task<Course> AddEntityAsync(Course course)
        {
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return course;
        }

        public async Task<Course?> GetCourseByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Courses
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        }

        public async Task<(IEnumerable<Course> Items, int TotalCount)> GetCoursesAsync(
            int page, int pageSize, string? category = null, string? search = null)
        {
            var query = _context.Courses
                .Where(c => !c.IsDeleted && c.Status == CourseStatus.Published)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(c => c.Category == category);

            IQueryable<Course> orderedQuery;
            if (!string.IsNullOrWhiteSpace(search))
            {
                var tsQueryString = string.Join(" & ", search.Trim()
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(w => w + ":*"));
                query = query.Where(c => c.SearchVector.Matches(EF.Functions.ToTsQuery("english", tsQueryString)));
                orderedQuery = query.OrderByDescending(c => c.SearchVector.RankCoverDensity(EF.Functions.ToTsQuery("english", tsQueryString)));
            }
            else
            {
                orderedQuery = query.OrderByDescending(c => c.CreatedAt);
            }

            var totalCount = await query.CountAsync();
            var items = await orderedQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<IEnumerable<Course>> GetByInstructorIdAsync(int instructorId, CancellationToken cancellationToken)
        {
            return await _context.Courses
                .Where(c => c.InstructorId == instructorId && !c.IsDeleted)
                .AsNoTracking()
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task UpdateAsync(Course entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            _context.Courses.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
