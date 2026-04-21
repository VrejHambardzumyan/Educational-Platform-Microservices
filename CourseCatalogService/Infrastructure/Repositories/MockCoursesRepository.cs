using CourseCatalogService.Infrastructure.Entities;
using CourseCatalogService.Infrastructure.Interfaces;
using System.Collections.Concurrent;

namespace CourseCatalogService.Infrastructure.Repositories
{
    public class MockCoursesRepository : ICourseRepository
    {
        private readonly ConcurrentDictionary<int, Course> _courses = new();
        private int _nextId = 1;

        public Task<Course> AddEntityAsync(Course entity)
        {
            entity.Id = Interlocked.Increment(ref _nextId);
            _courses[entity.Id] = entity;
            return Task.FromResult(entity);
        }

        public Task<(IEnumerable<Course> Items, int TotalCount)> GetCoursesAsync(
            int page, int pageSize, string? category = null, string? search = null)
        {
            var query = _courses.Values.Where(c => !c.IsDeleted).AsEnumerable();
            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(c => c.Category == category);
            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(c => c.Title.Contains(search) || c.Description.Contains(search));
            var all = query.ToList();
            var items = all.Skip((page - 1) * pageSize).Take(pageSize);
            return Task.FromResult((items.AsEnumerable(), all.Count));
        }

        public Task<Course?> GetCourseByIdAsync(int id, CancellationToken cancellationToken)
        {
            _courses.TryGetValue(id, out var course);
            return Task.FromResult(course != null && !course.IsDeleted ? course : null);
        }

        public Task<IEnumerable<Course>> GetByInstructorIdAsync(int instructorId, CancellationToken cancellationToken)
        {
            var courses = _courses.Values.Where(c => c.InstructorId == instructorId && !c.IsDeleted);
            return Task.FromResult(courses.AsEnumerable());
        }

        public Task UpdateAsync(Course entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            _courses[entity.Id] = entity;
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync() => Task.CompletedTask;
    }
}
