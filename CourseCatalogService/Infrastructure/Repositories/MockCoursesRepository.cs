using CourseCatalogService.Infrastructure.Entities;
using CourseCatalogService.Infrastructure.Interfaces;
using System.Collections.Concurrent;

namespace CourseCatalogService.Infrastructure.Repositories
{
    public class MockCoursesRepository : ICourseRepository
    {
        private readonly ConcurrentDictionary<string, Course> _course = new();

        public Task<Course> AddEntityAsync(Course entity)
        {
            _course[entity.Title] = entity;
            return Task.FromResult(entity);
        }

        public Task<IEnumerable<Course>> GetAllCoursesAsync()
        {
            return Task.FromResult<IEnumerable<Course>>(_course.Values);
        }

        public Task<Course?> GetCourseByNameAsync(string courseName, CancellationToken cancellationToken)
        {
            _course.TryGetValue(courseName, out var course);
            return Task.FromResult(course);
        }
    }
}
