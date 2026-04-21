using CourseCatalogService.Application.Models.DTOs;

namespace CourseCatalogService.Application.Interfaces
{
    public interface IContentBlockService
    {
        Task<IEnumerable<ContentBlockResponseDto>> GetByLessonIdAsync(int lessonId, CancellationToken cancellationToken = default);
        Task<ContentBlockResponseDto> AddAsync(int lessonId, ContentBlockRequestDto dto, CancellationToken cancellationToken = default);
        Task<ContentBlockResponseDto?> UpdateAsync(int id, ContentBlockRequestDto dto, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task ReorderAsync(int lessonId, List<int> orderedBlockIds, CancellationToken cancellationToken = default);
    }
}
