using CourseEnrollment.Application.Models.DTOs;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace CourseEnrollment.Application.ExternalCalls.CouseCatalog
{
    public class CourseCatalogClient : ICourseCatalogClient
    {
        private readonly HttpClient _httpClient;
        private readonly CourseCatalogSettings _settings;
        public CourseCatalogClient(HttpClient httpClient,IOptions<CourseCatalogSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
        }

        public async Task<decimal> GetCoursePriceAsync(int courseId, CancellationToken cancellationToken = default)
        {
            var endpoint = _settings.Endpoints.GetCourseById.Replace("{courseId}", courseId.ToString());

            var token = GenerateServiceToken();
            _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync(endpoint, cancellationToken);
            response.EnsureSuccessStatusCode();

            var course = await response.Content.ReadFromJsonAsync<CourseCatalogResponse>(cancellationToken: cancellationToken);

            return course!.Price;
        }

        private static string GenerateServiceToken()
        {
            var rsa = RSA.Create();
            rsa.ImportFromPem(File.ReadAllText("C:\\Users\\ideat\\Documents\\private_key.pem"));

            var key = new Microsoft.IdentityModel.Tokens.RsaSecurityKey(rsa);
            var creds = new Microsoft.IdentityModel.Tokens.SigningCredentials(
                key, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.RsaSha256);

            var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
                issuer: "AuthService",
                audience: "CourseCatalogService",
                expires: DateTime.UtcNow.AddMinutes(5),
                signingCredentials: creds
            );

            return new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
