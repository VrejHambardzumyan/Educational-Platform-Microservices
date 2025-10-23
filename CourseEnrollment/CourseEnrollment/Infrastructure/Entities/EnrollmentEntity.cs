using CourseEnrollment.Infrastructure.Status;

namespace CourseEnrollment.Infrastructure.Entities
{
    public class EnrollmentEntity
    {
        public int Id { get; set; }
        
        public int UserId { get; set; }
        
        public int CourseId { get; set; }

        public DateTime CreatedAt { get; set; }

        public decimal Amount { get; set; }

        public DateTime? ActivatedAt { get; set; }

        public string? Status { get; set; }
    }
}
