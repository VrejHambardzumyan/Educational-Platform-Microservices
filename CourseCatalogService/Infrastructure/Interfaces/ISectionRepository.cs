using CourseCatalogService.Infrastructure.Entities;

namespace CourseCatalogService.Infrastructure.Interfaces
{
    public interface ISectionRepository
    {
        Task<Section> AddAsync(Section section, CancellationToken cancellationToken = default);
        Task<IEnumerable<Section>> GetByCourseIdAsync(int courseId, CancellationToken cancellationToken = default);
        Task<Section?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task UpdateAsync(Section section, CancellationToken cancellationToken = default);
        Task DeleteAsync(Section section, CancellationToken cancellationToken = default);
    }
}
