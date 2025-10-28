namespace CourseEnrollment.Application.ExternalCalls.CouseCatalog
{
    public class CourseCatalogSettings
    {
        public string BaseUrl { get; set; } = string.Empty;
        public EndpointsConfig Endpoints { get; set; } = new EndpointsConfig();

        public class EndpointsConfig
        {
            public string GetCourseById {  get; set; } = string.Empty;
        }

    }
}
