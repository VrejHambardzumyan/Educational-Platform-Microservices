using CourseCatalogService.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace CourseCatalogService.Infrastructure.Seeders
{
    public class CourseSeeder
    {
        public static async Task SeedAsync(CourseDbContext context)
        {
            if (await context.Courses.AnyAsync())
                return; // already seeded, skip

            var courses = new List<Course>
            {
                new Course
                {
                    Title = "CSharp_Fundamentals",
                    Description = "Learn the core concepts of C# programming including OOP, LINQ, and async/await.",
                    DurationInMonth = 3,
                    Price = 4999
                },
                new Course
                {
                    Title = "ASP_NET_Core_WebAPI",
                    Description = "Build production-ready REST APIs with ASP.NET Core, EF Core, and JWT authentication.",
                    DurationInMonth = 4,
                    Price = 6999
                },
                new Course
                {
                    Title = "React_From_Scratch",
                    Description = "Master React from components and hooks to state management and API integration.",
                    DurationInMonth = 3,
                    Price = 5499
                },
                new Course
                {
                    Title = "Microservices_Architecture",
                    Description = "Design and build scalable microservices using .NET, Docker, and message brokers.",
                    DurationInMonth = 6,
                    Price = 9999
                },
                new Course
                {
                    Title = "PostgreSQL_For_Developers",
                    Description = "Deep dive into PostgreSQL, indexing, query optimization, and EF Core integration.",
                    DurationInMonth = 2,
                    Price = 3499
                },
                new Course
                {
                    Title = "Docker_And_Kubernetes",
                    Description = "Containerize applications with Docker and orchestrate them with Kubernetes.",
                    DurationInMonth = 3,
                    Price = 7499
                },
                new Course
                {
                    Title = "Clean_Architecture_DDD",
                    Description = "Apply Clean Architecture and Domain Driven Design principles in real .NET projects.",
                    DurationInMonth = 5,
                    Price = 8499
                },
                new Course
                {
                    Title = "TypeScript_Advanced",
                    Description = "Go beyond the basics with generics, decorators, utility types, and design patterns.",
                    DurationInMonth = 2,
                    Price = 3999
                }
            };

            await context.Courses.AddRangeAsync(courses);
            await context.SaveChangesAsync();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[Seeder] {courses.Count} courses seeded successfully.");
            Console.ResetColor();
        }
    }
}
