using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using UserManagementService.Application.Interfaces;
using UserManagementService.Application.Models.JwtOption;
using UserManagementService.Infrastructure.Entities;

namespace UserManagementService.Application.Services
{
    public class RSATokenService : ITokenService
    {
        private readonly JwtOptions _options;
        private readonly RSA _rsaPrivate;
        private readonly RsaSecurityKey _privateKey;

        public RSATokenService(IOptions<JwtOptions> options)
        {
            _options = options.Value;

            if (string.IsNullOrEmpty(_options.PrivateKeyPath))
                throw new Exception("Private key path not configured");

            _rsaPrivate = RSA.Create();
            _rsaPrivate.ImportFromPem(File.ReadAllText(_options.PrivateKeyPath));   
            _privateKey = new RsaSecurityKey(_rsaPrivate);
        }
        /// <summary>
        /// Generates a signed JWT containing the user's role (for backward-compatible role checks)
        /// and individual permission names (for fine-grained <c>HasPermission</c> checks).
        /// </summary>
        public string GenerateAccessToken(User user)
        {
            var roleName = user.UserRole?.Name ?? "Student";

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("userId", user.Id.ToString()),
                // ClaimTypes.Role keeps [Authorize(Roles="...")] working in every service
                new Claim(ClaimTypes.Role, roleName)
            };

            var permissions = user.UserRole?.RolePermissions
                .Select(rp => rp.Permission.Name)
                ?? [];

            claims.AddRange(permissions.Select(p => new Claim("permissions", p)));

            var creds = new SigningCredentials(_privateKey, SecurityAlgorithms.RsaSha256);

            var token = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,  // List<Claim> is accepted by the constructor
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }
}
