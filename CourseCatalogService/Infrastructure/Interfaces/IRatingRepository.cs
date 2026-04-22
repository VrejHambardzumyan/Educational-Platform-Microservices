using CourseCatalogService.Infrastructure.Entities;

namespace CourseCatalogService.Infrastructure.Interfaces
{
    public interface IRatingRepository
    {
        Task<CourseRating?> GetByUserAndCourseAsync(int userId, int courseId);
        Task<IEnumerable<CourseRating>> GetByCourseAsync(int courseId);
        Task AddAsync(CourseRating rating);
        Task UpdateAsync(CourseRating rating);
        Task<(double Average, int Count)> RecalculateAsync(int courseId);
    }
}
