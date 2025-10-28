namespace CourseEnrollment.Application.Models.DTOs
{
    public class CreateEnrollmentRequestDto
    {
        public int UserId { get; set; }

        public int CourseId { get; set; }
    }
}
