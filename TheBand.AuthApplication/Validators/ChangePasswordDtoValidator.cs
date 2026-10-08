using FluentValidation;
using TheBand.AuthApplication.Dtos;

namespace TheBand.AuthApplication.Validators;

public sealed class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
{
    public ChangePasswordDtoValidator()
    {
        RuleFor(user => user.CurrentPassword).NotEmpty();

        RuleFor(user => user.NewPassword).NotEmpty().MinimumLength(8).MaximumLength(100);

        RuleFor(user => user.ConfirmNewPassword).NotEmpty()
            .Equal(user => user.NewPassword).WithMessage("As senhas não batem.");
    }
}
