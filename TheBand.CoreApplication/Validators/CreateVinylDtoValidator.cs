using FluentValidation;
using TheBand.CoreApplication.Dtos;

namespace TheBand.CoreApplication.Validators;

public sealed class CreateVinylDtoValidator : AbstractValidator<CreateVinylDto>
{
    public CreateVinylDtoValidator()
    {
        RuleFor(x => x.Artist)
          .NotEmpty().WithMessage("Artista é obrigatório")
          .MinimumLength(3).WithMessage("Artista deve ter pelo menos 3 caracteres")
          .MaximumLength(50).WithMessage("Artista não deve exceder 50 caracteres");

        RuleFor(x => x.Album)
            .NotEmpty().WithMessage("Álbum é obrigatório")
            .MinimumLength(3).WithMessage("Álbum deve ter pelo menos 3 caracteres")
            .MaximumLength(50).WithMessage("Álbum não deve exceder 50 caracteres");

        RuleFor(x => x.Year)
            .NotEmpty().WithMessage("Ano é obrigatório")
            .InclusiveBetween(1900, DateTime.Now.Year).WithMessage($"Ano deve ser entre 1900 e {DateTime.Now.Year}");

        RuleFor(x => x.Price)
            .NotEmpty().WithMessage("Preço é obrigatório")
            .GreaterThan(0).WithMessage("Preço deve ser maior que 0");

        RuleFor(x => x.Photo)
            .NotEmpty().WithMessage("Foto é obrigatória")
            .MinimumLength(10).WithMessage("URL da foto deve ter pelo menos 10 caracteres")
            .MaximumLength(255).WithMessage("URL da foto não deve exceder 255 caracteres");
    }
}
