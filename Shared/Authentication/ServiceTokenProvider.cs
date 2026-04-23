using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace Shared.Authentication;

public class ServiceTokenProvider(IConfiguration configuration) : IServiceTokenProvider
{
    private readonly string _privateKeyPath = configuration["ServiceAuth:PrivateKeyPath"]
        ?? throw new InvalidOperationException("ServiceAuth:PrivateKeyPath is not configured.");
    private readonly string _issuer = configuration["ServiceAuth:Issuer"] ?? "AuthService";
    private readonly string _audience = configuration["ServiceAuth:Audience"] ?? "LMSPlatform";

    public string GenerateToken()
    {
        var rsa = RSA.Create();
        rsa.ImportFromPem(File.ReadAllText(_privateKeyPath));

        var key = new RsaSecurityKey(rsa);
        var creds = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            expires: DateTime.UtcNow.AddMinutes(5),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
