using CourseCatalogService.Infrastructure.Entities;

namespace CourseCatalogService.Infrastructure.Interfaces
{
    public interface ILessonRepository
    {
        Task<Lesson> AddAsync(Lesson lesson);
        Task<IEnumerable<Lesson>> GetByCourseIdAsync(int courseId, CancellationToken cancellationToken = default);
        Task<Lesson?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task UpdateAsync(Lesson lesson);
        Task<bool> DeleteAsync(int id);
        Task<int> CountByCourseIdAsync(int courseId, CancellationToken cancellationToken = default);
    }
}
