using ExpenseTracker.DTO;
using ExpenseTracker.Models;

namespace ExpenseTracker.Services;

public interface IJwtService
{
    JwtTokenResponse CreateToken(User user);
}