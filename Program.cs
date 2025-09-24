using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using System.Security.Cryptography;
using UserManagementService.Application.Interfaces;
using UserManagementService.Application.Services;
using UserManagementService.Infrastructure;
using UserManagementService.Infrastructure.Interfaces;
using UserManagementService.Infrastructure.Repositories;
using UserManagementService.Presentation.JwtOption;

namespace UserManagementService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddPostgresDbContext(builder.Configuration);
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            //builder.Services.AddScoped<IUserRepository, MockUserRepository>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddSingleton<RSATokenService>();

            builder.Services.AddOpenApi();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var publicKeyPath = builder.Configuration["JwtSettings:PublicKeyPath"];
            Console.WriteLine($"DEBUG: PublicKeyPath = {publicKeyPath}");

            if (string.IsNullOrEmpty(publicKeyPath))
                throw new Exception("Public key path not found in configuration.");

            using var rsa = RSA.Create();
            rsa.ImportFromPem(File.ReadAllText(publicKeyPath));

            builder.Services
                .AddAuthentication("Bearer")
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new RsaSecurityKey(rsa)
                    };
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
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
