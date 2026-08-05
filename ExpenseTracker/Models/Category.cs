namespace ExpenseTracker.Models;

public class Category
{
    public int Id { get; private set; }
    public int? UserId { get; set; }
    public User User { get; set; } = null!;
    public string Name { get; set; }
    public string? Description { get; set; }
    public List<Transaction> Transactions { get; private set; } =  new();

    public Category(int? userId, string name, string? description)
    {
        UserId = userId;
        Name = name;
        Description = description;
    }
}