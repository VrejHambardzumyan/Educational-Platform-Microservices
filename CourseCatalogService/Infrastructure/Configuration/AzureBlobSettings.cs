namespace CourseCatalogService.Infrastructure.Configuration
{
    public class AzureBlobSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public int SasTokenExpiryMinutes { get; set; } = 60;
    }
}
