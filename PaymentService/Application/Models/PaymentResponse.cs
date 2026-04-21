namespace PaymentService.Application.Models
{
    public class PaymentResponse
    {
        public Guid PaymentId { get; set; }
        public int UserId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? ChargeId { get; set; }
    }

    public class PaymentStatusResponse
    {
        public Guid PaymentId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? ChargeId { get; set; }
        public string? FailureReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
    }
}
