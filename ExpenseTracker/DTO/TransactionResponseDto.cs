using ExpenseTracker.Models;

namespace ExpenseTracker.DTO;

public class TransactionResponseDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public DateTime Date { get; set; }
    public string? Category { get; set; }
}