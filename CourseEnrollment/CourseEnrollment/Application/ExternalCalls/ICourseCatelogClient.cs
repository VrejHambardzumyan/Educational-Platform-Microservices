namespace CourseEnrollment.Application.Interfaces
{
    public interface ICourseCatalogClient
    {
        Task<decimal> GetCoursePriceAsync(int courseId, CancellationToken cancellationToken = default);
    }
}
