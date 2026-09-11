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

        services.AddScoped<IValidator<CreateVinylDto>, CreateVinylDtoValidator>();

        services.AddScoped<IValidator<UpdateVinylDto>, UpdateVinylDtoValidator>();

        return services;
    }
}
