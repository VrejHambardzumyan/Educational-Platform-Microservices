using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace CourseCatalogService.Infrastructure.Configuration
{
    public static class JwtAuthenticationExtension
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var publicKey = File.ReadAllText("C:\\Users\\Karen\\public_key.pem");
            var rsa = RSA.Create();
            rsa.ImportFromPem(publicKey);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidIssuer = "AuthService",
                        ValidateAudience = false,
                        ValidAudience = "CourseCatalogService",
                        ValidateLifetime = false,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new RsaSecurityKey(rsa)
                    };

                    #region 

                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("❌ JWT Authentication failed:");
                            Console.WriteLine(context.Exception.Message);
                            Console.ResetColor();

                            
                            // Console.WriteLine(context.Exception);

                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("✅ Token successfully validated for user: " +
                                context.Principal?.Identity?.Name);
                            Console.ResetColor();
                            return Task.CompletedTask;
                        },
                        OnMessageReceived = context =>
                        {
                            var authHeader = context.Request.Headers["Authorization"].ToString();
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"🔍 Authorization header received: '{authHeader}'");
                            Console.ResetColor();

                            return Task.CompletedTask;
                        },

                    };
                    #endregion 
                });
        return services;
        }
    }
}
