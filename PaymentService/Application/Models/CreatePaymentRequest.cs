namespace PaymentService.Application.Models
{
    public class CreatePaymentRequest
    {
        public int UserId { get; set; }
        public Guid PaymentId { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
