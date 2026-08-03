using ExpenseTracker.DTO;
using ExpenseTracker.Models;

namespace ExpenseTracker.Services;

public interface IUserService
{
    ServiceResult<UserResponseDto> Login(LoginRequestDto request);
    ServiceResult<UserResponseDto> Register(RegisterRequestDto request);
    ServiceResult<bool> Logout();
}