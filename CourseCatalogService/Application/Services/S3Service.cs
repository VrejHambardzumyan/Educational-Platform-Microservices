using Amazon.S3;
using Amazon.S3.Model;
using CourseCatalogService.Application.Interfaces;
using CourseCatalogService.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace CourseCatalogService.Application.Services
{
    public class S3Service(IAmazonS3 s3Client, IOptions<S3Settings> options) : IS3Service
    {
        private readonly IAmazonS3 _s3Client = s3Client;
        private readonly S3Settings _settings = options.Value;

        private string VideoKey(int courseId, int lessonId) =>
            $"courses/{courseId}/lessons/{lessonId}/video.mp4";

        public string GeneratePresignedUploadUrl(int courseId, int lessonId)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _settings.BucketName,
                Key = VideoKey(courseId, lessonId),
                Verb = HttpVerb.PUT,
                Expires = DateTime.UtcNow.AddMinutes(_settings.PresignedUrlExpiryMinutes),
                ContentType = "video/mp4"
            };

            return _s3Client.GetPreSignedURL(request);
        }

        public string BuildPublicVideoUrl(int courseId, int lessonId) =>
            $"https://{_settings.BucketName}.s3.{_settings.Region}.amazonaws.com/{VideoKey(courseId, lessonId)}";
    }
}
