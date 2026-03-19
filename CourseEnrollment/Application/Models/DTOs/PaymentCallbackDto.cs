namespace CourseEnrollment.Application.Models.DTOs
{
    public class PaymentCallbackDto
    {
        public Guid PaymentId { get; set; }
        public bool IsSuccess { get; set; }
        public string? FailureReason { get; set; }
    }
}
