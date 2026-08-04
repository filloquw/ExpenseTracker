using ExpenseTracker.DTO;
using FluentValidation;

namespace ExpenseTracker.Validators;

public class UpdateTransactionValidator : AbstractValidator<UpdateTransactionRequestDto>
{
    public UpdateTransactionValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Название транзакции не должно превышать 100 символов")
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .When(x => x.Name != null);
        
        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Описание транзакции не должно превышать 500 символов")
            .When(x => x.Description != null);
        
        RuleFor(x=>x.Amount)
            .GreaterThan(0).WithMessage("Сумма обязательна и должна быть положительна.")
            .When(x => x.Amount.HasValue);
        
        RuleFor(x=>x.Date)
            .LessThanOrEqualTo(_=DateTime.UtcNow).WithMessage("Дата транзакции не может быть позже текущей.")
            .When(x => x.Date.HasValue);
    }
}