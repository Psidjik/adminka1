using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using User.Domain;
using User.Domain.Data;

namespace User.Application;

public class JwtService(UserDbContext context, IConfiguration configuration) : IJwtService
{
    private readonly string _issuer = configuration["JwtProperties:Issuer"] ?? string.Empty;
    private readonly string _audience = configuration["JwtProperties:Audience"] ?? string.Empty;
    private readonly byte[] _token = Encoding.UTF8.GetBytes(configuration["JwtProperties:TOKEN"] ?? string.Empty);
    
    public string GenerateAccessToken(Teacher userAccount)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userAccount.Id.ToString())
        };

        var key = new SymmetricSecurityKey(_token);

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var tokenDescriptor = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    public async Task SaveRefreshToken(Guid userId, string refreshToken)
    {
        var refreshTokenEntity = new RefreshToken(userId, refreshToken, DateTimeOffset.UtcNow.AddDays(7));

        context.RefreshTokens.Add(refreshTokenEntity);
        await context.SaveChangesAsync();
    }
    
    
    public async Task<string?> RefreshAccessToken(string refreshToken)
    {
        var existingToken = await context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.ExpiresAt > DateTimeOffset.UtcNow);

        if (existingToken is null)
            return null;

        var userAccount = await context.Teachers.FirstOrDefaultAsync(u => u.Id == existingToken.TeacherId);
        return userAccount == null ? null : GenerateAccessToken(userAccount);
    }

    public async Task<string> ValidateAccessToken(string accessToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var principal = tokenHandler.ValidateToken(accessToken, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(_token),
            ValidateIssuer = true,
            ValidIssuer = _issuer,
            ValidateAudience = true,
            ValidAudience = _audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        }, out var validatedToken);

        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return userId ?? throw new SecurityTokenException("User ID not found in token");
    }
    
    public  Guid? GetUserIdFromAccessToken(string accessToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = _token;

        var principal = tokenHandler.ValidateToken(accessToken, new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _issuer,
            ValidAudience = _audience,
            IssuerSigningKey = new SymmetricSecurityKey(key),
        }, out var _);

        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}
