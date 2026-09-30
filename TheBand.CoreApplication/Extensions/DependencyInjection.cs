using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TheBand.CoreApplication.Dtos;
using TheBand.CoreApplication.Interfaces;
using TheBand.CoreApplication.Services;
using TheBand.CoreApplication.Validators;

namespace TheBand.CoreApplication.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddCoreApplication(this IServiceCollection services)
    {
        services.AddScoped<IVinylService, VinylService>();

        services.AddScoped<ICassetteService, CassetteService>();

        services.AddScoped<IConcertService, ConcertService>();

        services.AddScoped<IValidator<CreateVinylDto>, CreateVinylDtoValidator>();

        services.AddScoped<IValidator<UpdateVinylDto>, UpdateVinylDtoValidator>();

        services.AddScoped<IValidator<CreateCassetteDto>, CreateCassetteDtoValidator>();

        services.AddScoped<IValidator<UpdateCassetteDto>, UpdateCassetteDtoValidator>();

        services.AddScoped<IValidator<CreateConcertDto>, CreateConcertDtoValidator>();

        services.AddScoped<IValidator<UpdateConcertDto>, UpdateConcertDtoValidator>();

        return services;
    }
}
