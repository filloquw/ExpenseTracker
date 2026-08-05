namespace ExpenseTracker.Exceptions;

public class BusinessException : Exception
{
    public BusinessException(string message, string? code = null) : base(message)
    {
        Code = code;
    }
    
    public string? Code { get; }
}