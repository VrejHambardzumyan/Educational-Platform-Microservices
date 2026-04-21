using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using UserManagementService.Application.Interfaces;
using UserManagementService.Application.Models.JwtOption;
using UserManagementService.Application.Services;
using UserManagementService.Infrastructure;
using UserManagementService.Infrastructure.Configuration;
using UserManagementService.Infrastructure.Interfaces;
using UserManagementService.Infrastructure.Repositories;
using UserManagementService.Infrastructure.Seeders;

namespace UserManagementService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddPostgresDbContext(builder.Configuration);

            builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtSettings"));
            builder.Services.AddJwtAuthentication(builder.Configuration);
            builder.Services.AddAuthorization();

            builder.Services.Configure<TwoFactorSettings>(builder.Configuration.GetSection("TwoFactor"));
            builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));

            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddSingleton<ITokenService, RSATokenService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IOtpService, OtpService>();
            builder.Services.AddScoped<IEmailService, EmailService>();

            builder.Services.AddFluentValidationAutoValidation()
                            .AddFluentValidationClientsideAdapters();
            builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "User Management API", Version = "v1" });
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
                var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
                await db.Database.MigrateAsync();
                await UserSeeder.SeedAsync(db);
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
