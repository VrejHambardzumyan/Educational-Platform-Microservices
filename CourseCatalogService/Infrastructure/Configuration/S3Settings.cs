namespace CourseCatalogService.Infrastructure.Configuration
{
    public class S3Settings
    {
        public string BucketName { get; set; } = string.Empty;
        public string Region { get; set; } = "us-east-1";
        public int PresignedUrlExpiryMinutes { get; set; } = 15;
    }
}
