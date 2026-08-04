using ExpenseTracker.DTO;
using FluentValidation;

namespace ExpenseTracker.Validators;

public class CreateCategoryValidator : AbstractValidator<CreateCategoryRequestDto>
{
    public CreateCategoryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Название категории обязательно")
            .MaximumLength(50).WithMessage("Название категории не должно превышать 50 символов");
        
        RuleFor(x => x.Description)
            .MaximumLength(250).WithMessage("Описание категории не должна превышать 250 символов");
    }
}