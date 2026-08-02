namespace ExpenseTracker.Models;

public class User
{
    public int Id { get; private set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; private set; }
    public decimal Balance { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public List<Transaction> Transactions { get; private set; }

    public User(string email, string username, string passwordHash)
    {
        Email = email;
        Username = username;
        PasswordHash = passwordHash;
    }
}