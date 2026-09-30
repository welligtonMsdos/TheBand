using FluentValidation;
using TheBand.CoreApplication.Dtos;

namespace TheBand.CoreApplication.Validators;

public sealed class UpdateConcertDtoValidator : AbstractValidator<UpdateConcertDto>
{
    public UpdateConcertDtoValidator()
    {
        RuleFor(x => x.Artist)
            .NotEmpty().WithMessage("Artista é obrigatório")
            .MinimumLength(3).WithMessage("Artista deve ter pelo menos 3 caracteres")
            .MaximumLength(50).WithMessage("Artista não deve exceder 50 caracteres");

        RuleFor(x => x.Venue)
            .NotEmpty().WithMessage("Local é obrigatório")
            .MinimumLength(3).WithMessage("Local deve ter pelo menos 3 caracteres")
            .MaximumLength(50).WithMessage("Local não deve exceder 50 caracteres");

        RuleFor(x => x.ShowDate)
            .NotEqual(default(DateOnly)).WithMessage("Data do show é obrigatória");

        RuleFor(x => x.Photo)
            .NotEmpty().WithMessage("Foto é obrigatória")
            .MinimumLength(3).WithMessage("URL da foto deve ter pelo menos 3 caracteres")
            .MaximumLength(255).WithMessage("URL da foto não deve exceder 255 caracteres");
    }
}
