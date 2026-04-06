using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using StudentManagementAPI.DTOs;

namespace StudentManagementAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<AuthService> _logger;

        // Demo users (in production, store in DB with hashed passwords)
        private readonly Dictionary<string, string> _users = new(StringComparer.OrdinalIgnoreCase)
        {
            { "admin", "Admin@123" },
            { "user",  "User@123"  }
        };

        public AuthService(IConfiguration config, ILogger<AuthService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public AuthResponseDto? Login(LoginDto dto)
        {
            _logger.LogInformation("Login attempt for user: {Username}", dto.Username);

            if (!_users.TryGetValue(dto.Username, out var storedPassword) ||
                storedPassword != dto.Password)
            {
                _logger.LogWarning("Invalid credentials for user: {Username}", dto.Username);
                return null;
            }

            var token = GenerateJwtToken(dto.Username);
            var expiry = DateTime.UtcNow.AddHours(
                double.Parse(_config["Jwt:ExpiryHours"] ?? "2"));

            _logger.LogInformation("Token generated successfully for user: {Username}", dto.Username);

            return new AuthResponseDto
            {
                Token = token,
                Username = dto.Username,
                ExpiresAt = expiry
            };
        }

        private string GenerateJwtToken(string username)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, username.Equals("admin", StringComparison.OrdinalIgnoreCase)
                    ? "Admin" : "User"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat,
                    DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer64)
            };

            var expiry = DateTime.UtcNow.AddHours(
                double.Parse(_config["Jwt:ExpiryHours"] ?? "2"));

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expiry,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
