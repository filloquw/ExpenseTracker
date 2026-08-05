using ExpenseTracker.Models;

namespace ExpenseTracker.DTO;

public class UpdateTransactionRequestDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Amount { get; set; }
    public DateTime? Date { get; set; }
    public TransactionType? Type { get; set; }
    public int? CategoryId { get; set; }
    public bool RemoveCategory { get; set; }
}