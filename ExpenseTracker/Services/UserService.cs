using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ExpenseTracker.Data;
using ExpenseTracker.DTO;
using ExpenseTracker.Exceptions;
using ExpenseTracker.Models;

namespace ExpenseTracker.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly IJwtService _jwtService;

    public UserService(ApplicationDbContext context,  IJwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    private JwtTokenResponse CreateUserToken(User user)
    {
        var token  = _jwtService.CreateToken(user);
        _context.UserTokens.Add(new UserToken(
            user.Id,
            token.Jti,
            token.ExpiresAt));
        
        _context.SaveChanges();
        return token;
    }
    
    public AuthResponseDto Login(LoginRequestDto request)
    {
        var user = _context.Users.SingleOrDefault(u => u.Email == request.Email.ToLower());

        if (user == null)
            throw new BusinessException("Пользователь не найден");
        
        var passwordCorrect = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

        if (!passwordCorrect)
            throw new BusinessException("Пароль неверный");

        var token = CreateUserToken(user);

        return new AuthResponseDto
        {
            Token = token.Token,
            Username = user.Username,
            Email = user.Email
        };
    }

    public AuthResponseDto Register(RegisterRequestDto request)
    {
        var exists = _context.Users.Any(u => u.Email == request.Email.ToLower());
        if (exists)
            throw new BusinessException($"Уже существует пользователь, зарегистрированный под email: {request.Email}");
        
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User(
            request.Email,
            request.Username,
            passwordHash);
        
        _context.Users.Add(user);
        _context.SaveChanges();
        var token = CreateUserToken(user);

        return new AuthResponseDto
        {
            Email = user.Email,
            Username = user.Username,
            Token = token.Token,
        };
    }

    public void Logout(string? jti)
    {
        var token = _context.UserTokens.FirstOrDefault(t=>t.Jti==jti);
        
        if (token == null)
            throw new BusinessException("Токен не найден");
        
        token.IsRevoked = true;
        token.RevokedAt = DateTime.UtcNow;
        _context.SaveChanges();
    }
}