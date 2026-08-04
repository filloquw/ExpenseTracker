using ExpenseTracker.DTO;
using FluentValidation;

namespace ExpenseTracker.Validators;

public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryRequestDto>
{
    public UpdateCategoryValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(50).WithMessage("Название категории не должно превышать 50 символов")
            .Must(name => !string.IsNullOrWhiteSpace(name)).WithMessage("Название категории не может быть пустым")
            .When(x => x.Name != null);
        
        RuleFor(x => x.Description)
            .MaximumLength(250).WithMessage("Описание категории не должно превышать 250 символов")
            .When(x => x.Description != null);
    }
}