using CourseEnrollment.Application.ExternalCalls;
using CourseEnrollment.Application.ExternalCalls.CouseCatalog;
using CourseEnrollment.Application.ExternalCalls.Payment;
using CourseEnrollment.Application.Interfaces;
using CourseEnrollment.Application.Services;
using CourseEnrollment.Infrastructure;
using CourseEnrollment.Infrastructure.Configuration;
using CourseEnrollment.Infrastructure.Interfaces;
using CourseEnrollment.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace CourseEnrollment
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.Configure<CourseCatalogSettings>(
                    builder.Configuration.GetSection("ExternalServices:CourseCatalog"));
            builder.Services.AddHttpClient<ICourseCatalogClient, CourseCatalogClient>((sp, client) =>
            {
                var settings = sp.GetRequiredService<IOptions<CourseCatalogSettings>>().Value;
                client.BaseAddress = new Uri(settings.BaseUrl);
            });

            builder.Services.Configure<PaymentServiceSettings>(
                    builder.Configuration.GetSection("ExternalServices:Payment"));
            builder.Services.AddHttpClient<IPaymentServiceClient, PaymentServiceClient>((sp, client) =>
            {
                var settings = sp.GetRequiredService<IOptions<PaymentServiceSettings>>().Value;
                client.BaseAddress = new Uri(settings.BaseUrl);
            });

            builder.Services.AddControllers();
            builder.Services.AddPostgresDbContext(builder.Configuration);

            builder.Services.AddJwtAuthentication(builder.Configuration);
            builder.Services.AddAuthorization();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Enrollment API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

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

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<EnrollmentDbContext>();
                await db.Database.MigrateAsync();
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("AllowFrontend");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
