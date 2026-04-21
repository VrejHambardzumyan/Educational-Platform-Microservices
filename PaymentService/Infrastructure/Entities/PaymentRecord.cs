namespace PaymentService.Infrastructure.Entities
{
    public class PaymentRecord
    {
        public int Id { get; set; }
        public Guid PaymentId { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = PaymentStatus.Pending;
        public string? ChargeId { get; set; }
        public string? FailureReason { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessedAt { get; set; }
    }

    public static class PaymentStatus
    {
        public const string Pending = "Pending";
        public const string Processing = "Processing";
        public const string Completed = "Completed";
        public const string Failed = "Failed";
    }
}
