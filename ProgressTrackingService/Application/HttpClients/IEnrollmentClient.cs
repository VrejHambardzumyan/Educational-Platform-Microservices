namespace ProgressTrackingService.Application.HttpClients
{
    public interface IEnrollmentClient
    {
        /// <summary>
        /// Returns true when the current user has an active (paid) enrollment for
        /// <paramref name="courseId"/>. Forwards the caller's Bearer token so
        /// CourseEnrollmentService can identify the user.
        /// </summary>
        Task<bool> IsEnrolledAsync(int courseId, CancellationToken ct = default);
    }
}
