using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TheBand.CoreApplication.Services;
using TheBand.CoreApplication.Validators;

namespace TheBand.CoreApplication.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddCoreApplication(this IServiceCollection services)
    {        
        services.Scan(scan => scan
                .FromAssemblyOf<VinylService>()
                .AddClasses(classes => classes.Where(t => t.Name.EndsWith("Service")))
                .AsMatchingInterface()
                .WithScopedLifetime());

        services.AddValidatorsFromAssembly(typeof(CreateVinylDtoValidator).Assembly);       

        return services;
    }
}
