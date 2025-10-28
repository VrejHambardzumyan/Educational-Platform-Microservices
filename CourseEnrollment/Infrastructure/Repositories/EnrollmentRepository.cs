using CourseEnrollment.Infrastructure.Entities;
using CourseEnrollment.Infrastructure.Interfaces;
using CourseEnrollment.Infrastructure.Status;
using Microsoft.EntityFrameworkCore;

namespace CourseEnrollment.Infrastructure.Repositories
{
    public class EnrollmentRepository(EnrollmentDbContext dbContext) : IEnrollmentRepository
    {
        public readonly EnrollmentDbContext _context = dbContext;

        public async Task<EnrollmentEntity> AddEnrollmentAsync(EnrollmentEntity entity, CancellationToken cancellationToken = default)
        {
            _context.Enrollments.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return entity;
        }

        public async Task<IEnumerable<EnrollmentEntity>> GetAllByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.Enrollments.Where(e => e.UserId == userId).ToListAsync(cancellationToken);
        }

        public async Task<EnrollmentEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Enrollments.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        public async Task MarkAsDeletedAsync(int enrollmentId, CancellationToken cancellationToken = default)
        {
            var enrollment = await GetByIdAsync(enrollmentId, cancellationToken);

            if (enrollment == null)
            {
                throw new KeyNotFoundException($"Enrollment with ID {enrollmentId} was not found");
            }

            enrollment!.Status = nameof(PaymentStatus.Deleted);

            await _context.SaveChangesAsync(cancellationToken);

        }

        public async Task MarkAsPaidAsync(int enrollmentId,CancellationToken cancellationToken = default)
        {
            var enrollment = await GetByIdAsync(enrollmentId, cancellationToken);

            if (enrollment == null)
            {
                throw new KeyNotFoundException($"Enrollment with ID {enrollmentId} was not found");
            }
            enrollment!.ActivatedAt = DateTime.UtcNow;
            enrollment!.Status = nameof(PaymentStatus.Completed);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
