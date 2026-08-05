using ExpenseTracker.Models;

namespace ExpenseTracker.Services;

public interface IJwtService
{
    string CreateToken(User user);
}