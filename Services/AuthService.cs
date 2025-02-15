using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using azspx_mgt_web.Models;
using Microsoft.IdentityModel.Tokens;

namespace azspx_mgt_web.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly Dictionary<string, UserInfo> _users;

    public AuthService(IConfiguration configuration)
    {
        _configuration = configuration;
        // In a real application, this would come from a database
        _users = new Dictionary<string, UserInfo>
        {
            {
                "admin", new UserInfo
                {
                    Username = "admin",
                    // For testing purposes, using plain text password
                    PasswordHash = "admin123", // WARNING: In production, use proper password hashing
                    Roles = new[] { "Admin" }
                }
            }
        };
    }

    public async Task<LoginResponse?> AuthenticateAsync(LoginRequest request)
    {
        if (!ValidateUser(request, out var user) || user == null)
        {
            return null;
        }

        var token = GenerateJwtToken(user);
        return new LoginResponse
        {
            Token = token,
            Username = user.Username,
            Expiration = DateTime.UtcNow.AddDays(
                int.Parse(_configuration["Jwt:ExpiryInDays"] ?? "7"))
        };
    }

    public string GenerateJwtToken(UserInfo user)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? 
                throw new InvalidOperationException("JWT Key not configured")));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Username),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Name, user.Username)
        };

        claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(
                int.Parse(_configuration["Jwt:ExpiryInDays"] ?? "7")),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public bool ValidateUser(LoginRequest request, out UserInfo? user)
    {
        // Simple string comparison for testing
        // In production, use proper password hashing and verification
        if (_users.TryGetValue(request.Username, out user))
        {
            return user.PasswordHash == request.Password;
        }

        user = null;
        return false;
    }
}