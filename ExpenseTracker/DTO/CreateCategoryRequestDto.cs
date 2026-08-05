namespace ExpenseTracker.DTO;

public class CreateCategoryRequestDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}