namespace PaymentService.Infrastructure.Configuration
{
    public class EnrollmentCallbackSettings
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string WebhookSecret { get; set; } = string.Empty;
    }
}
