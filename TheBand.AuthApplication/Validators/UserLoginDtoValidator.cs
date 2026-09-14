using FluentValidation;
using TheBand.AuthApplication.Dtos;

namespace TheBand.AuthApplication.Validators;

public sealed class UserLoginDtoValidator : AbstractValidator<UserLoginDto>
{
    public UserLoginDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório.")
            .EmailAddress().WithMessage("Email inválido.")
            .MaximumLength(50).WithMessage("Email deve ter no máximo 50 caracteres.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Senha é obrigatória.")
            .MinimumLength(8).WithMessage("Senha deve ter pelo menos 8 caracteres.")
            .MaximumLength(128).WithMessage("Senha deve ter no máximo 128 caracteres.");
    }
}
