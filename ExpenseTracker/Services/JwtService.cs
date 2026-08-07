using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ExpenseTracker.DTO;
using ExpenseTracker.Models;
using Microsoft.IdentityModel.Tokens;

namespace ExpenseTracker.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public JwtTokenResponse CreateToken(User user)
    {
        var jti = Guid.NewGuid().ToString();
        var claims = new[]
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                user.Id.ToString()),
            new Claim(
                ClaimTypes.Email,
                user.Email),
            new Claim(
                JwtRegisteredClaimNames.Jti, jti)
        };
        
        var jwtKey = _configuration["Jwt:Key"] ?? throw new Exception("JWT Key not found");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var expiresMinutes = _configuration["Jwt:ExpiryMinutes"] ?? throw new Exception("JWT ExpireMinutes not found");
        var expireAt = DateTime.UtcNow.AddMinutes(int.Parse(expiresMinutes));
        
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expireAt,
            signingCredentials: creds);

        return new JwtTokenResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Jti = jti,
            ExpiresAt = expireAt
        };
    }
}