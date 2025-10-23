using CourseEnrollment.Application.Models.DTOs;

namespace CourseEnrollment.Application.Interfaces
{
    public interface IEnrollmentService
    {
        Task<EnrollmentResponseDto> AddEnrollmentAsync(CreatEnrollmentRequestDto requestDto, CancellationToken cancellationToken = default);

        Task MarkAsPaidAsync(int enrollmentId,CancellationToken cancellationToken = default);

        Task MarkAsDeletedAsync(int enrollmentId, CancellationToken cancellationToken = default);

        Task<IEnumerable<EnrollmentResponseDto>> GetAllByUserIdAsync(int userId, CancellationToken cancellationToken = default);

        Task<EnrollmentResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    }
}
