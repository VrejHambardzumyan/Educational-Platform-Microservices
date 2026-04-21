namespace CourseCatalogService.Application.Interfaces
{
    public interface IBlobStorageService
    {
        Task<string> UploadAsync(Stream fileStream, string fileName, string containerName, string contentType, CancellationToken cancellationToken = default);
        Task DeleteAsync(string blobUrl, string containerName, CancellationToken cancellationToken = default);
        string GetFileUrl(string blobName, string containerName);
        string RefreshSasUrl(string storedUrl, string containerName);
    }
}
