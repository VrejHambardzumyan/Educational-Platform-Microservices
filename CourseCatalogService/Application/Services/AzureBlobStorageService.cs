using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using CourseCatalogService.Application.Interfaces;
using CourseCatalogService.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace CourseCatalogService.Application.Services
{
    public class AzureBlobStorageService(IOptions<AzureBlobSettings> options) : IBlobStorageService
    {
        private readonly BlobServiceClient _client = new(options.Value.ConnectionString);
        private readonly AzureBlobSettings _settings = options.Value;

        public async Task<string> UploadAsync(Stream fileStream, string fileName, string containerName, string contentType, CancellationToken cancellationToken = default)
        {
            var container = _client.GetBlobContainerClient(containerName);
            await container.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

            var blobName = $"{Guid.NewGuid()}/{fileName}";
            var blob = container.GetBlobClient(blobName);

            await blob.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: cancellationToken);

            return GetFileUrl(blobName, containerName);
        }

        public async Task DeleteAsync(string blobUrl, string containerName, CancellationToken cancellationToken = default)
        {
            var blobUriBuilder = new BlobUriBuilder(new Uri(blobUrl));
            var container = _client.GetBlobContainerClient(containerName);
            var blob = container.GetBlobClient(blobUriBuilder.BlobName);
            await blob.DeleteIfExistsAsync(cancellationToken: cancellationToken);
        }

        public string RefreshSasUrl(string storedUrl, string containerName)
        {
            var blobName = new BlobUriBuilder(new Uri(storedUrl)).BlobName;
            return GetFileUrl(blobName, containerName);
        }

        // Returns a time-limited read SAS URL. blobName is the path within the container.
        public string GetFileUrl(string blobName, string containerName)
        {
            var container = _client.GetBlobContainerClient(containerName);
            var blob = container.GetBlobClient(blobName);

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerName,
                BlobName = blobName,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(_settings.SasTokenExpiryMinutes)
            };
            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            return blob.GenerateSasUri(sasBuilder).ToString();
        }
    }
}
