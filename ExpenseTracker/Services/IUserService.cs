using System.Security.Claims;
using ExpenseTracker.DTO;
using ExpenseTracker.Models;

namespace ExpenseTracker.Services;

public interface IUserService
{
    AuthResponseDto Login(LoginRequestDto request);
    AuthResponseDto Register(RegisterRequestDto request);
    void Logout(string? jti);
}