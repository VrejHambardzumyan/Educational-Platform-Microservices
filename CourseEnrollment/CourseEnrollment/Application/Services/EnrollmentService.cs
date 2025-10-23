using CourseEnrollment.Application.ExternalCalls;
using CourseEnrollment.Application.Interfaces;
using CourseEnrollment.Application.Models.DTOs;
using CourseEnrollment.Infrastructure;
using CourseEnrollment.Infrastructure.Entities;
using CourseEnrollment.Infrastructure.Interfaces;
using CourseEnrollment.Infrastructure.Status;

namespace CourseEnrollment.Application.Services
{
    public class EnrollmentService(CourseCatalogClient catalogClient, IEnrollmentRepository enrollmentRepo) : IEnrollmentService
    {
        private readonly CourseCatalogClient _catalogClient = catalogClient;
        private readonly IEnrollmentRepository _enrollmentRepo = enrollmentRepo;

        public async Task<EnrollmentResponseDto> AddEnrollmentAsync(CreatEnrollmentRequestDto requestDtoEntity, CancellationToken cancellationToken = default)
        {
            var price = await _catalogClient.GetCoursePriceAsync(requestDtoEntity.CourseId, cancellationToken);
            var enrollment = new EnrollmentEntity
            {
                UserId = requestDtoEntity.UserId,
                CourseId = requestDtoEntity.CourseId,
                CreatedAt = DateTime.UtcNow,
                Amount = price,
                Status = nameof(PaymentStatus.Draft)
            };

            var created = await _enrollmentRepo.AddEnrollmentAsync(enrollment, cancellationToken);

            return new EnrollmentResponseDto
            {
                Id = enrollment.Id,
                UserId = enrollment.UserId,
                CourseId =enrollment.CourseId,
                CreatedAt = enrollment.CreatedAt,
                Amount = enrollment.Amount,
                ActivatedAt = enrollment.ActivatedAt,
                Status = enrollment.Status,
            };
        }

        public async Task<IEnumerable<EnrollmentResponseDto>> GetAllByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            var enrollments = await _enrollmentRepo.GetAllByUserIdAsync(userId, cancellationToken);

            var result = enrollments.Select(e => new EnrollmentResponseDto
            {
                Id = e.Id,
                UserId = e.UserId,
                CourseId = e.CourseId,
                CreatedAt = e.CreatedAt,
                Amount = e.Amount,
                ActivatedAt = e.ActivatedAt,
                Status = e.Status
            });
            return result;
        }

        public async Task<EnrollmentResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var enrollmetnt = await _enrollmentRepo.GetByIdAsync(id, cancellationToken);

            if (enrollmetnt == null)
            {
                return null;
            }

            return new EnrollmentResponseDto
            {
                Id = enrollmetnt.Id,
                UserId = enrollmetnt.UserId,
                CourseId = enrollmetnt.CourseId,
                CreatedAt = enrollmetnt.CreatedAt,
                Amount = enrollmetnt.Amount,
                ActivatedAt = enrollmetnt.ActivatedAt,
                Status = enrollmetnt.Status

            };
        }

        public async Task MarkAsDeletedAsync(int enrollmentId, CancellationToken cancellationToken = default)
        {
            await _enrollmentRepo.MarkAsDeletedAsync(enrollmentId, cancellationToken);
        }

        public async Task MarkAsPaidAsync(int enrollmentId, CancellationToken cancellationToken = default)
        {
            await _enrollmentRepo.MarkAsPaidAsync(enrollmentId, cancellationToken);
        }
    }
}
