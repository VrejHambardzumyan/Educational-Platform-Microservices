namespace CourseCatalogService.Application.Interfaces
{
    public interface IS3Service
    {
        string GeneratePresignedUploadUrl(int courseId, int lessonId);
        string BuildPublicVideoUrl(int courseId, int lessonId);
    }
}
