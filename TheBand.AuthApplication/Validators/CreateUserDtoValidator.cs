using FluentValidation;
using TheBand.AuthApplication.Dtos;

namespace TheBand.AuthApplication.Validators;

public sealed class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserDtoValidator()
    {
        RuleFor(user => user.Name)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(100)
            .WithMessage("O nome deve ter entre 3 e 100 caracteres.");

        RuleFor(user => user.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(50)
            .WithMessage("O email é inválido.");

        RuleFor(user => user.Password)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(100)
            .WithMessage("A senha deve ter entre 8 e 100 caracteres.");

        RuleFor(user => user.Role)
            .IsInEnum()
            .WithMessage("A função é inválida.");
    }
}
