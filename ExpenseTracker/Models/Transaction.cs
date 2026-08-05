namespace ExpenseTracker.Models;

public class Transaction
{
    public int Id { get; private set; }
    public int? UserId { get; private set; }
    public User User { get; private set; } = null!;
    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public DateTime CreatedAt { get; private set; }
    public TransactionType Type { get; set; }
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }

    public Transaction(int? userId, string name, string? description, decimal amount, DateTime date, TransactionType type, int? categoryId)
    {
        UserId = userId;
        Name = name;
        Description = description;
        Amount = amount;
        Date = date;
        Type = type;
        CategoryId = categoryId;
    }
}