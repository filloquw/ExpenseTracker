using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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
    public string CreateToken(User user)
    {
        var claims = new[]
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                user.Id.ToString()),
            new Claim(
                ClaimTypes.Email,
                user.Email),
        };
        
        var jwtKey = _configuration["Jwt:Key"] ?? throw new Exception("JWT Key not found");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var expiresMinutes = _configuration["Jwt:ExpiryMinutes"] ?? throw new Exception("JWT ExpireMinutes not found");
        
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(expiresMinutes)),
            signingCredentials: creds);
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}