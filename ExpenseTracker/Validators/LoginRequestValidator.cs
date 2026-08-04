using ExpenseTracker.DTO;
using FluentValidation;

namespace ExpenseTracker.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress().WithMessage("Некорректный формат email")
            .MaximumLength(255).WithMessage("Email не может быть длиннее 255 символов");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MaximumLength(100).WithMessage("Пароль не может быть длиннее 100 символов");
    }
}