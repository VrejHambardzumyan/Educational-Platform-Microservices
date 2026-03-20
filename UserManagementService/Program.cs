using FluentValidation;
using FluentValidation.AspNetCore;
using System.Reflection;
using UserManagementService.Application.Interfaces;
using UserManagementService.Application.Models.JwtOption;
using UserManagementService.Application.ModelValidation;
using UserManagementService.Application.Services;
using UserManagementService.Infrastructure;
using UserManagementService.Infrastructure.Interfaces;
using UserManagementService.Infrastructure.Repositories;

namespace UserManagementService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddPostgresDbContext(builder.Configuration);
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddSingleton<ITokenService, RSATokenService>();

            builder.Services.AddScoped<IUserRepository, UserRepository>();
            //builder.Services.AddSingleton<IUserRepository, MockUserRepository>();

            builder.Services.AddFluentValidationAutoValidation()
                            .AddFluentValidationClientsideAdapters();
            builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());


            builder.Services.AddOpenApi();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtSettings"));
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var privateKeyPath = builder.Configuration["JwtSettings:PrivateKeyPath"];
            //Console.WriteLine($"DEBUG: PrivateKeyPath = {privateKeyPath}");

            if (string.IsNullOrEmpty(privateKeyPath))
                throw new Exception("Private key path not found in configuration.");

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

            app.UseCors();
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
