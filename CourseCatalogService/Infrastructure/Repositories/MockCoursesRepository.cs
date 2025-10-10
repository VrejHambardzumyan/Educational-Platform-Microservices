using CourseCatalogService.Infrastructure.Entities;
using CourseCatalogService.Infrastructure.Interfaces;
using System.Collections.Concurrent;

namespace CourseCatalogService.Infrastructure.Repositories
{
    public class MockCoursesRepository : ICourseRepository
    {
        private readonly ConcurrentDictionary<int, Course> _course = new();

        public Task<Course> AddEntityAsync(Course entity)
        {
            _course[entity.Id] = entity;
            return Task.FromResult(entity);
        }

        public Task<IEnumerable<Course>> GetAllCoursesAsync()
        {
            return Task.FromResult<IEnumerable<Course>>(_course.Values);
        }

        public Task<Course?> GetCourseByIdAsync(int id, CancellationToken cancellationToken)
        {
            _course.TryGetValue(id, out var course);
            return Task.FromResult(course);
        }
    }
}
