namespace ExpenseTracker.DTO;

public class CategoryResponseDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
}