using CourseCatalogService.Application.Models.DTOs;

namespace CourseCatalogService.Application.Interfaces
{
    public interface ISectionService
    {
        Task<SectionResponseDto> CreateAsync(int courseId, SectionRequestDto dto, int instructorId, bool isAdmin = false, CancellationToken cancellationToken = default);
        Task<IEnumerable<SectionResponseDto>> GetByCourseIdAsync(int courseId, CancellationToken cancellationToken = default);
        Task<LessonResponseDto> AddLessonAsync(int sectionId, LessonRequestDto dto, int instructorId, bool isAdmin = false, CancellationToken cancellationToken = default);
        Task<IEnumerable<LessonResponseDto>> GetLessonsAsync(int sectionId, CancellationToken cancellationToken = default);
    }
}
