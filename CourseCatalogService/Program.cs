using CourseCatalogService.Application.Interfaces;
using CourseCatalogService.Application.Services;
using CourseCatalogService.Infrastructure;
using CourseCatalogService.Infrastructure.Configuration;
using CourseCatalogService.Infrastructure.Interfaces;
using CourseCatalogService.Infrastructure.Repositories;
using CourseCatalogService.Infrastructure.Seeders;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace CourseCatalogService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // CORS — must be registered in services first
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

            builder.Services.AddJwtAuthentication(builder.Configuration);
            builder.Services.AddAuthorization();
            builder.Services.AddControllers();
            builder.Services.AddPostgresDbContext(builder.Configuration);
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Catalog API", Version = "v1" });
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
                        new string[] {}
                    }
                });
            });

            builder.Services.AddScoped<ICourseRepository, CourseRepository>();
            builder.Services.AddScoped<ICourseService, CourseService>();

            builder.Services.AddFluentValidationAutoValidation()
                            .AddFluentValidationClientsideAdapters();
            builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<CourseDbContext>();
                await db.Database.MigrateAsync();  // runs migrations automatically
                await CourseSeeder.SeedAsync(db);  // seeds if empty
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // ORDER MATTERS — CORS must come before Auth
            app.UseCors("AllowFrontend");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}