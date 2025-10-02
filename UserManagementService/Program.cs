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
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddSingleton<ITokenService, RSATokenService>();

             builder.Services.AddScoped<IUserRepository, UserRepository>();
            //builder.Services.AddSingleton<IUserRepository, MockUserRepository>();

            builder.Services.AddOpenApi();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtSettings"));
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var privateKeyPath = builder.Configuration["JwtSettings:PrivateKeyPath"];
            //Console.WriteLine($"DEBUG: PrivateKeyPath = {privateKeyPath}");

            if (string.IsNullOrEmpty(privateKeyPath))
                throw new Exception("Private key path not found in configuration.");

            ///for Jwt validation, must be in another service

            //using var rsa = RSA.Create();
            //rsa.ImportFromPem(File.ReadAllText(PublicKeyPath));

            //builder.Services
            //    .AddAuthentication("Bearer")
            //    .AddJwtBearer(options =>
            //    {
            //        options.TokenValidationParameters = new TokenValidationParameters
            //        {
            //            ValidateIssuer = true,
            //            ValidateAudience = true,
            //            ValidateIssuerSigningKey = true,
            //            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            //            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            //            IssuerSigningKey = new RsaSecurityKey(rsa)
            //        };
            //    });

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
