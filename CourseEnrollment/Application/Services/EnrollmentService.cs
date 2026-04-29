using CourseEnrollment.Application.ExternalCalls.CouseCatalog;
using CourseEnrollment.Application.ExternalCalls.Payment;
using CourseEnrollment.Application.Interfaces;
using CourseEnrollment.Application.Models.DTOs;
using CourseEnrollment.Application.Validation;
using CourseEnrollment.Infrastructure.Entities;
using CourseEnrollment.Infrastructure.Interfaces;
using CourseEnrollment.Infrastructure.Status;

namespace CourseEnrollment.Application.Services
{
    public class EnrollmentService(
        IPaymentServiceClient paymentClient,
        ICourseCatalogClient catalogClient,
        IEnrollmentRepository enrollmentRepo) : IEnrollmentService
    {
        private readonly IPaymentServiceClient _paymentClient = paymentClient;
        private readonly ICourseCatalogClient _catalogClient = catalogClient;
        private readonly IEnrollmentRepository _enrollmentRepo = enrollmentRepo;

        public async Task<EnrollmentResponseDto?> AddEnrollmentAsync(CreateEnrollmentRequestDto requestDtoEntity, CancellationToken cancellationToken = default)
        {
            var hasActive = await _enrollmentRepo.HasActiveEnrollmentAsync(
                requestDtoEntity.UserId, requestDtoEntity.CourseId, cancellationToken);
            if (hasActive)
                return null;

            var price = await _catalogClient.GetCoursePriceAsync(requestDtoEntity.CourseId, cancellationToken);
            var enrollment = new EnrollmentEntity
            {
                UserId = requestDtoEntity.UserId,
                CourseId = requestDtoEntity.CourseId,
                CreatedAt = DateTime.UtcNow,
                Amount = price,
                Status = price == 0
                    ? nameof(PaymentStatus.Completed)
                    : nameof(PaymentStatus.Draft)
                ActivatedAt = price == 0 ? DateTime.UtcNow : null
            };

            var created = await _enrollmentRepo.AddEnrollmentAsync(enrollment, cancellationToken);

            return MapToDto(created);
        }

        public async Task<IEnumerable<EnrollmentResponseDto>> GetAllByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            var enrollments = await _enrollmentRepo.GetAllByUserIdAsync(userId, cancellationToken);
            return enrollments.Select(MapToDto);
        }

        public async Task<EnrollmentResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _enrollmentRepo.GetByIdAsync(id, cancellationToken);
            return entity == null ? null : MapToDto(entity);
        }

        public async Task MarkAsDeletedAsync(int enrollmentId, CancellationToken cancellationToken = default)
        {
            await _enrollmentRepo.MarkAsDeletedAsync(enrollmentId, cancellationToken);
        }

        public async Task MarkAsPaidAsync(int enrollmentId, CancellationToken cancellationToken = default)
        {
            await _enrollmentRepo.MarkAsPaidAsync(enrollmentId, cancellationToken);
        }

        public async Task<Guid> SubmitCardAsync(int userId, SubmitCardRequestDto cardDto, CancellationToken cancellationToken)
        {
            if (!CardValidator.IsValidLuhn(cardDto.CardNumber))
                throw new ArgumentException("Invalid card number.");

            if (!CardValidator.IsValidExpiry(cardDto.ExpiryMonth, cardDto.ExpiryYear))
                throw new ArgumentException("Card has expired.");

            if (!CardValidator.IsValidCvv(cardDto.Cvv))
                throw new ArgumentException("Invalid CVV.");

            var paymentId = Guid.NewGuid();

            // Atomically claim all Draft enrollments — concurrent calls get 0 rows and fail fast
            var claimedCount = await _enrollmentRepo.SetProcessingAsync(userId, paymentId, cancellationToken);
            if (claimedCount == 0)
                throw new InvalidOperationException("No draft enrollments to pay for.");

            var processing = await _enrollmentRepo.GetAllByPaymentIdAsync(paymentId, cancellationToken);
            var totalAmount = processing.Sum(e => e.Amount);

            // PaymentService fires POST /CourseEnrollment/PaymentCallback when done
            await _paymentClient.CreatPaymentAsync(userId, paymentId, totalAmount, cancellationToken);

            return paymentId;
        }

        public async Task HandlePaymentCallbackAsync(Guid paymentId, bool isSuccess, CancellationToken cancellationToken = default)
        {
            var enrollments = await _enrollmentRepo.GetAllByPaymentIdAsync(paymentId, cancellationToken);

            if (!enrollments.Any())
                throw new KeyNotFoundException($"No enrollments found for PaymentId {paymentId}");

            foreach (var enrollment in enrollments)
            {
                if (isSuccess)
                {
                    enrollment.Status = nameof(PaymentStatus.Completed);
                    enrollment.ActivatedAt = DateTime.UtcNow;
                }
                else
                {
                    enrollment.Status = nameof(PaymentStatus.Failed);
                }
            }

            await _enrollmentRepo.SaveChangesAsync(cancellationToken);
        }

        private static EnrollmentResponseDto MapToDto(EnrollmentEntity e) => new()
        {
            Id = e.Id,
            UserId = e.UserId,
            CourseId = e.CourseId,
            CreatedAt = e.CreatedAt,
            Amount = e.Amount,
            PaymentId = e.PaymentId,
            ActivatedAt = e.ActivatedAt,
            Status = e.Status
        };
    }
}
