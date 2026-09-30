using FluentValidation;
using TheBand.AuthApplication.Dtos;

namespace TheBand.AuthApplication.Validators;

public sealed class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserDtoValidator()
    {
        RuleFor(user => user.Name).NotEmpty().MinimumLength(3).MaximumLength(100);

        RuleFor(user => user.Email).NotEmpty().EmailAddress().MaximumLength(50);

        RuleFor(user => user.Password).NotEmpty().MinimumLength(8).MaximumLength(100);

        RuleFor(user => user.Role).IsInEnum();
    }
}
