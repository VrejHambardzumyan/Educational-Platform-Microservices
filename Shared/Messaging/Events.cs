namespace Shared.Messaging
{
    public record EnrollmentCompletedEvent(int UserId, int CourseId, string CourseTitle);
    public record PaymentFailedEvent(int UserId, Guid PaymentId, string Reason);
    public record NewLessonEvent(int CourseId, string CourseTitle, string LessonTitle);
    public record ProgressMilestoneEvent(int UserId, int CourseId, string CourseTitle, int Percentage);
    public record CourseCompletedEvent(int UserId, int CourseId, string CourseTitle);
}
