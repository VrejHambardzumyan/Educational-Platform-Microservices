using CourseCatalogService.Infrastructure.Entities;

namespace CourseCatalogService.Infrastructure.Interfaces
{
    public interface ICourseRepository
    {
        Task<Course> AddEntityAsync(Course entity);
        Task<(IEnumerable<Course> Items, int TotalCount)> GetCoursesAsync(int page, int pageSize, string? category = null, string? search = null);
        Task<Course?> GetCourseByIdAsync(int id, CancellationToken cancellationToken);
        Task<IEnumerable<Course>> GetByInstructorIdAsync(int instructorId, CancellationToken cancellationToken);
        Task UpdateAsync(Course entity);
        Task SaveChangesAsync();
    }
}
