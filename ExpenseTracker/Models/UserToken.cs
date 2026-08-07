namespace ExpenseTracker.Models;

public class UserToken
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public string Jti { get; set; } = null!;
    public bool IsRevoked { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? RevokedAt { get; set; }

    public UserToken(int userId, string jti, DateTime expiresAt)
    {
        UserId = userId;
        Jti = jti;
        IsRevoked = false;
        ExpiresAt = expiresAt;
        CreatedAt = DateTime.UtcNow;
        RevokedAt = null;
    }
}