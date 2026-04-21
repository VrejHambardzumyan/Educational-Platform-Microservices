namespace CourseCatalogService.Application.Models.DTOs
{
    public class UploadResponseDto
    {
        public required string Url { get; init; }
        public string? FileName { get; init; }
        public long? FileSizeBytes { get; init; }
        public string? ContentType { get; init; }
    }
}
