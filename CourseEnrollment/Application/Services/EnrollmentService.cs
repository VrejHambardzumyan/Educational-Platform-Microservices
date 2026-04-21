using CourseEnrollment.Application.ExternalCalls.CouseCatalog;
using CourseEnrollment.Application.ExternalCalls.Payment;
using CourseEnrollment.Application.Interfaces;
using CourseEnrollment.Application.Models.DTOs;
using CourseEnrollment.Application.Validation;
using CourseEnrollment.Infrastructure;
using CourseEnrollment.Infrastructure.Entities;
using CourseEnrollment.Infrastructure.Interfaces;
using CourseEnrollment.Infrastructure.Status;
using Microsoft.EntityFrameworkCore;

namespace CourseEnrollment.Application.Services
{
    public class EnrollmentService(
        IPaymentServiceClient paymentClient,
        ICourseCatalogClient catalogClient,
        IEnrollmentRepository enrollmentRepo,
        IServiceScopeFactory scopeFactory) : IEnrollmentService
    {
        private readonly IPaymentServiceClient _paymentClient = paymentClient;
        private readonly ICourseCatalogClient _catalogClient = catalogClient;
        private readonly IEnrollmentRepository _enrollmentRepo = enrollmentRepo;
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

        public async Task<EnrollmentResponseDto> AddEnrollmentAsync(CreateEnrollmentRequestDto requestDtoEntity, CancellationToken cancellationToken = default)
        {
            var hasActive = await _enrollmentRepo.HasActiveEnrollmentAsync(
                requestDtoEntity.UserId, requestDtoEntity.CourseId, cancellationToken);
            if (hasActive)
                throw new InvalidOperationException("User already has an active enrollment for this course.");

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
                CourseId = enrollment.CourseId,
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

        public async Task<Guid> SubmitCardAsync(int userId, SubmitCardRequestDto cardDto, CancellationToken cancellationToken)
        {
            if (!CardValidator.IsValidLuhn(cardDto.CardNumber))
                throw new ArgumentException("Invalid card number.");

            if (!CardValidator.IsValidExpiry(cardDto.ExpiryMonth, cardDto.ExpiryYear))
                throw new ArgumentException("Card has expired.");

            if (!CardValidator.IsValidCvv(cardDto.Cvv))
                throw new ArgumentException("Invalid CVV.");

            var userEnrollments = await _enrollmentRepo.GetAllByUserIdAsync(userId, cancellationToken);
            var draftEnrollments = userEnrollments.Where(e => e.Status == "Draft").ToList();

            if (draftEnrollments.Count == 0)
                throw new InvalidOperationException("No draft enrollments to pay for.");

            var paymentId = Guid.NewGuid();

            foreach (var e in draftEnrollments)
            {
                e.PaymentId = paymentId;
                e.Status = nameof(PaymentStatus.Processing);
            }

            await _enrollmentRepo.SaveChangesAsync(cancellationToken);

            var totalAmount = draftEnrollments.Sum(e => e.Amount);
            await _paymentClient.CreatPaymentAsync(userId, paymentId, totalAmount, cancellationToken);

            _ = Task.Run(async () =>
            {
                await Task.Delay(1500);
                var isSuccess = Random.Shared.NextDouble() > 0.1;

                using var scope = _scopeFactory.CreateScope();
                var svc = scope.ServiceProvider.GetRequiredService<IEnrollmentService>();
                await svc.HandlePaymentCallbackAsync(paymentId, isSuccess);
            });

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
    }
}
