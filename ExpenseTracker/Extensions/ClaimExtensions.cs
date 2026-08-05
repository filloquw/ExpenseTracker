using System.Security.Claims;

namespace ExpenseTracker.Extensions;

public static class ClaimExtensions
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            throw new UnauthorizedAccessException("Айди пользователя не найдено");
        
        return int.Parse(userId);
    }
}