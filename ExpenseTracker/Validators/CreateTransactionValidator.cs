using ExpenseTracker.DTO;
using FluentValidation;

namespace ExpenseTracker.Validators;

public class CreateTransactionValidator : AbstractValidator<CreateTransactionRequestDto>
{
    public CreateTransactionValidator()
    {
        RuleFor(x=>x.Name)
            .NotEmpty().WithMessage("Название транзакции не может быть пустым.")
            .MaximumLength(100).WithMessage("Название транзакции не должно превышать 100 символов");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Описание транзакции не должно превышать 500 символов");
        
        RuleFor(x=>x.Amount)
            .GreaterThan(0).WithMessage("Сумма обязательна и должна быть положительна.");
        
        RuleFor(x=>x.Date)
            .LessThanOrEqualTo(_=DateTime.UtcNow).WithMessage("Дата транзакции не может быть позже текущей.");
    }
}