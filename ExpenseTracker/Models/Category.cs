namespace ExpenseTracker.Models;

public class Category
{
    public int Id { get; private set; }
    public int? UserId { get; set; }
    public User User { get; set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public List<Transaction> Transactions { get; private set; }

    public Category(int? userId, string name, string? description)
    {
        UserId = userId;
        Name = name;
        Description = description;
    }
}