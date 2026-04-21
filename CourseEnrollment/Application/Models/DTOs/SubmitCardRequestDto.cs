namespace CourseEnrollment.Application.Models.DTOs
{
    public class SubmitCardRequestDto
    {
        public string CardNumber { get; set; } = string.Empty;
        public string CardholderName { get; set; } = string.Empty;
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        public string Cvv { get; set; } = string.Empty;
    }
}
