namespace ExpenseTracker.DTO;

public class JwtTokenResponse
{
    public string Token { get; init; } = null!;
    public string Jti { get; init; } = null!;
    public DateTime ExpiresAt { get; init; }
}