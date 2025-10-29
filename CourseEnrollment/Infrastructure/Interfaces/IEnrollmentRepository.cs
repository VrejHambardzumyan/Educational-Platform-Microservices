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
       

    }
}
