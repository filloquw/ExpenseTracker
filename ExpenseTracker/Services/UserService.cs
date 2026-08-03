using ExpenseTracker.Data;
using ExpenseTracker.DTO;
using ExpenseTracker.Models;

namespace ExpenseTracker.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public ServiceResult<UserResponseDto> Login(LoginRequestDto request)
    {
        var user = _context.Users.SingleOrDefault(u => u.Email == request.Email);

        if (user == null)
        {
            return new ServiceResult<UserResponseDto>
            {
                Success = false,
                Message = "Пользователя с таким email не существует."
            };
        }
        
        var passwordCorrect = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

        if (!passwordCorrect)
        {
            return new ServiceResult<UserResponseDto>
            {
                Success = false,
                Message = "Неверный пароль."
            };
        }

        return new ServiceResult<UserResponseDto>
        {
            Success = true,
            Data = new UserResponseDto
            {
                Email = user.Email,
                Username = user.Username,
                Balance = user.Balance
            }
        };
    }

    public ServiceResult<UserResponseDto> Register(RegisterRequestDto request)
    {
        var exists = _context.Users.Any(u => u.Email == request.Email);
        if (exists)
        {
            return new ServiceResult<UserResponseDto>
            {
                Success = false,
                Message = $"Уже существует пользователь, зарегистрированный под email: {request.Email}"
            };
        }
        
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User(
            request.Email,
            request.Username,
            passwordHash);
        
        _context.Users.Add(user);
        _context.SaveChanges();

        return new ServiceResult<UserResponseDto>
        {
            Success = true,
            Data = new UserResponseDto
            {
                Email = user.Email,
                Username = user.Username,
                Balance = null
            }
        };
    }

    public ServiceResult<bool> Logout()
    {
        throw new NotImplementedException();
    }
}