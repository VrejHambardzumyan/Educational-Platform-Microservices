using CourseCatalogService.Infrastructure.Entities;

namespace CourseCatalogService.Infrastructure.Interfaces
{
    public interface ICourseRepository
    {
        Task <Course> AddEntityAsync (Course entity);
        Task<IEnumerable<Course>> GetAllCoursesAsync();
        Task<Course?> GetCourseByIdAsync (int id, CancellationToken cancellationToken);
    }
}
