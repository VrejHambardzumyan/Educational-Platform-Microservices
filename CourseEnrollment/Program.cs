using CourseEnrollment.Application.ExternalCalls;
using CourseEnrollment.Application.ExternalCalls.CouseCatalog;
using CourseEnrollment.Application.ExternalCalls.Payment;
using CourseEnrollment.Application.Interfaces;
using CourseEnrollment.Application.Services;
using CourseEnrollment.Infrastructure;
using CourseEnrollment.Infrastructure.Interfaces;
using CourseEnrollment.Infrastructure.Repositories;
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

            builder.Services.Configure<PaymentServiceSettings>(
                    builder.Configuration.GetSection("ExternalServices:Payment"));
            builder.Services.AddHttpClient<IPaymentServiceClient, PaymentServiceClient>();


            builder.Services.AddControllers();
            builder.Services.AddPostgresDbContext(builder.Configuration);
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
            builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                    policy.WithOrigins(
                            "http://localhost:5173",
                            "http://localhost:5174",
                            "http://localhost:5175"
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod());
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors();
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
