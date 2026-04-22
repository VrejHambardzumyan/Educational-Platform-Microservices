using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace CourseEnrollment.Infrastructure.Configuration
{
    public static class JwtAuthenticationExtension
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var publicKey = File.ReadAllText("C:\\Users\\ideat\\Documents\\public_key.pem");
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
                    ValidateIssuer = true,
                    ValidIssuer = "AuthService",
                    ValidateAudience = true,
                    ValidAudience = "LMSPlatform",
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new RsaSecurityKey(rsa),
                    ClockSkew = TimeSpan.FromSeconds(30)
                };
            });

            return services;
        }
    }
}
