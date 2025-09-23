using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UserManagementService.Application.Interfaces;
using UserManagementService.Infrastructure;

namespace UserManagementService.Application.Services
{
    //ToDo - Research the topic about JWT token(access and refresh token) generations
    public class RSATokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly RsaSecurityKey _privateKey;
        private readonly RsaSecurityKey _publicKey;


        public RSATokenService(IConfiguration configuration)
        {
            _config = configuration;

            using var rsaPrivate = RSA.Create();
            rsaPrivate.ImportFromPem(File.ReadAllText(_config["Jwt:PrivateKeyPath"]));
            _privateKey = new RsaSecurityKey(rsaPrivate);

            using var rsaPublic = RSA.Create();
            rsaPublic.ImportFromPem(File.ReadAllText(_config["Jwt:PublicKeyPath"]));
            _publicKey = new RsaSecurityKey(rsaPublic);
        }
        public string GenerateAccessToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            
            var creds = new SigningCredentials(_privateKey, SecurityAlgorithms.RsaSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token); 
        }

        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }

        public SecurityKey GetPublicKey() => _publicKey; 
    }
}
