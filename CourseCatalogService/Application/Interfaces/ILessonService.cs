using CourseCatalogService.Application.Models.DTOs;

namespace CourseCatalogService.Application.Interfaces
{
    public interface ILessonService
    {
        Task<LessonResponseDto> AddLessonAsync(int courseId, LessonRequestDto dto, int instructorId);
        Task<IEnumerable<LessonResponseDto>> GetByCourseIdAsync(int courseId, CancellationToken cancellationToken = default);
        Task<LessonResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<LessonResponseDto?> UpdateLessonAsync(int id, LessonRequestDto dto, int instructorId);
        Task<bool> DeleteLessonAsync(int id, int instructorId, bool isAdmin);
    }
}
