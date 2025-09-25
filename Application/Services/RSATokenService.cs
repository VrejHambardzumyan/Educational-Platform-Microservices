using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using UserManagementService.Application.Interfaces;
using UserManagementService.Infrastructure.Entities;
using UserManagementService.Presentation.JwtOption;

namespace UserManagementService.Application.Services
{
    //ToDo - Research the topic about JWT token(access and refresh token) generations
    public class RSATokenService : ITokenService
    {
        private readonly JwtOptions _options;
        private readonly RSA _rsaPrivate;
        //private readonly RSA _rsaPublic;
        private readonly RsaSecurityKey _privateKey;
        //private readonly RsaSecurityKey _publicKey;


        public RSATokenService(IOptions<JwtOptions> options)
        {
            _options = options.Value;

            if (string.IsNullOrEmpty(_options.PrivateKeyPath))
                throw new Exception("Private key path not configured");

            _rsaPrivate = RSA.Create();
            _rsaPrivate.ImportFromPem(File.ReadAllText(_options.PrivateKeyPath));
            _privateKey = new RsaSecurityKey(_rsaPrivate);

            //if (string.IsNullOrEmpty(_config["Jwt:PublicKeyPath"]))
            //    throw new Exception("Public key path not configured");
            //_rsaPublic = RSA.Create();
            //_rsaPublic.ImportFromPem(File.ReadAllText(_config["Jwt:PublicKeyPath"]!));
            //_publicKey = new RsaSecurityKey(_rsaPublic);
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
                issuer: _options.Issuer,
                audience: _options.Audience,
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

        //public SecurityKey GetPublicKey() => _publicKey;
        //public void Dispose()
        //{
        //    _rsaPrivate?.Dispose();
        //    //_rsaPublic?.Dispose();
        //}
    }
}
