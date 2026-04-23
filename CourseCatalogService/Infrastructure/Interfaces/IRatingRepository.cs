using CourseCatalogService.Infrastructure.Entities;

namespace CourseCatalogService.Infrastructure.Interfaces
{
    public interface IRatingRepository
    {
        Task<CourseRating> UpsertAsync(int userId, int courseId, int rating, string? feedback, CancellationToken cancellationToken = default);
        Task<IEnumerable<CourseRating>> GetByCourseAsync(int courseId);
        Task<CourseRating?> GetByUserAndCourseAsync(int userId, int courseId);
        Task<(double Average, int Count)> RecalculateAsync(int courseId);
    }
}
