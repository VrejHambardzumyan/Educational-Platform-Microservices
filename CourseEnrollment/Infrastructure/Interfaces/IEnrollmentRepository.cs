using CourseEnrollment.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace CourseEnrollment.Infrastructure.Interfaces
{
    public interface IEnrollmentRepository
    {
        Task<EnrollmentEntity> AddEnrollmentAsync(EnrollmentEntity entity, CancellationToken cancellationToken = default);
       
        Task MarkAsPaidAsync(int enrollmentId, CancellationToken cancellationToken = default);
        
        Task MarkAsDeletedAsync(int enrollmentId, CancellationToken cancellationToken = default);
        
        Task<IEnumerable<EnrollmentEntity>> GetAllByUserIdAsync(int userId, CancellationToken cancellationToken = default);
        
        Task<EnrollmentEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task SaveChangesAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<EnrollmentEntity>> GetAllByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken = default);
        Task<bool> HasActiveEnrollmentAsync(int userId, int courseId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Atomically transitions all Draft enrollments for the user to Processing, assigning the given paymentId.
        /// Returns the number of rows affected (0 means no drafts existed).
        /// </summary>
        Task<int> SetProcessingAsync(int userId, Guid paymentId, CancellationToken cancellationToken = default);
    }
}
