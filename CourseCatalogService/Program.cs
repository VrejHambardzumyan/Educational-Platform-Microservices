
using CourseCatalogService.Application.Interfaces;
using CourseCatalogService.Application.Services;
using CourseCatalogService.Infrastructure;
using CourseCatalogService.Infrastructure.Interfaces;
using CourseCatalogService.Infrastructure.Repositories;

namespace CourseCatalogService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped<ICourseService, CourseService>();
            builder.Services.AddSingleton<ICourseRepository, MockCoursesRepository>();
            builder.Services.AddOpenApi();


            var app = builder.Build();
          
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
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
