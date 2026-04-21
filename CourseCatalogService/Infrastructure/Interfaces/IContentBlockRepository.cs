using CourseCatalogService.Infrastructure.Entities;

namespace CourseCatalogService.Infrastructure.Interfaces
{
    public interface IContentBlockRepository
    {
        Task<ContentBlock> AddAsync(ContentBlock block, CancellationToken cancellationToken = default);
        Task<ContentBlock?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<ContentBlock>> GetByLessonIdAsync(int lessonId, CancellationToken cancellationToken = default);
        Task UpdateAsync(ContentBlock block, CancellationToken cancellationToken = default);
        Task DeleteAsync(ContentBlock block, CancellationToken cancellationToken = default);
        Task UpdateRangeAsync(IEnumerable<ContentBlock> blocks, CancellationToken cancellationToken = default);
    }
}
