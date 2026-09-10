using FluentValidation;
using TheBand.AuthApplication.Dtos;

namespace TheBand.AuthApplication.Validators;

public class UserValidator : AbstractValidator<UserDto>
{
    public UserValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Nome não pode ser vazio!");
        RuleFor(x => x.Name).MinimumLength(3).WithMessage("Nome deve ter pelo menos 3 caracteres");
        RuleFor(x => x.Name).MaximumLength(100).WithMessage("Nome deve ter no máximo 100 caracteres");

        RuleFor(x => x.Email).NotEmpty().WithMessage("Email não pode ser vazio!");
        RuleFor(x => x.Email).EmailAddress().WithMessage("Email inválido!");
        RuleFor(x => x.Email).MinimumLength(12).WithMessage("Email deve ter pelo menos 12 caracteres");
        RuleFor(x => x.Email).MaximumLength(50).WithMessage("Email deve ter no máximo 50 caracteres");
    }
}
