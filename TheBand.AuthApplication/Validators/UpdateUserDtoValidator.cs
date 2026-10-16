using FluentValidation;
using TheBand.AuthApplication.Dtos;

namespace TheBand.AuthApplication.Validators;

public sealed class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserDtoValidator()
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

        RuleFor(user => user.Role)
            .IsInEnum()
            .WithMessage("A função é inválida.");
    }
}
