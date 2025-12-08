using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DevCompiler.Application.DTOs;
using DevCompiler.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DevCompiler.Infrastructure.Authentication;

public class JwtService : IAuthService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public JoinResponse GenerateToken(string nickname)
    {
        var userId = Guid.NewGuid().ToString();
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "SuperSuperSecretKeyThatIsAtLeast32CharactersLongForJWTTokenGeneration!");
        var issuer = _configuration["Jwt:Issuer"] ?? "DevCompiler";
        var audience = _configuration["Jwt:Audience"] ?? "DevCompilerUsers";

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("nickname", nickname),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
        };

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: signingCredentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new JoinResponse
        {
            Token = tokenString,
            UserId = userId,
            Nickname = nickname
        };
    }

    public string? ValidateToken(string token)
    {
        try
        {
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLongForJWTTokenGeneration!");
            var issuer = _configuration["Jwt:Issuer"] ?? "DevCompiler";
            var audience = _configuration["Jwt:Audience"] ?? "DevCompilerUsers";

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            return principal.FindFirst("nickname")?.Value;
        }
        catch
        {
            return null;
        }
    }
}