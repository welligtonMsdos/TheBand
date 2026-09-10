using Microsoft.Extensions.DependencyInjection;
using TheBand.AuthApplication.Interfaces;
using TheBand.AuthApplication.Services;

namespace TheBand.AuthApplication.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITokenService, TokenService>(); 

        return services;
    }
}
