using Microsoft.Extensions.Options;
using CourseEnrollment.Application.ExternalCalls;
using CourseEnrollment.Application.Interfaces;
using CourseEnrollment.Application.ExternalCalls;
using Microsoft.Extensions.Options;

namespace CourseEnrollment
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.Configure<CourseCatalogSettings>(
                    builder.Configuration.GetSection("ExternalServices:CourseCatalog"));

            builder.Services.AddHttpClient<ICourseCatalogClient, CourseCatalogClient>((sp, client) =>
            {
                var settings = sp.GetRequiredService<IOptions<CourseCatalogSettings>>().Value;
                client.BaseAddress = new Uri(settings.BaseUrl);
            });

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
