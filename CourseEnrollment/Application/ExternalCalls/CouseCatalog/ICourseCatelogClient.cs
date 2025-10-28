namespace CourseEnrollment.Application.ExternalCalls.CouseCatalog
{
    public interface ICourseCatalogClient
    {
        Task<decimal> GetCoursePriceAsync(int courseId, CancellationToken cancellationToken = default);
    }
}
