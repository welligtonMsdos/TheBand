using FluentValidation;
using TheBand.AuthApplication.Dtos;

namespace TheBand.AuthApplication.Validators;

public sealed class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserDtoValidator()
    {
        RuleFor(user => user.Name).NotEmpty().MinimumLength(3).MaximumLength(100);

        RuleFor(user => user.Email).NotEmpty().EmailAddress().MaximumLength(50);        

        RuleFor(user => user.Role).IsInEnum();
    }
}
