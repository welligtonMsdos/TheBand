using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TheBand.CoreDomain.Interfaces;
using TheBand.CoreInfrastructure.Repositories;

namespace TheBand.CoreInfrastructure.Data;

public static class DependencyInjection
{
    public static IServiceCollection AddCoreInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("CollectionConnection")
            ?? throw new InvalidOperationException("A conexão PostgreSQL 'ConnectionStrings:CollectionConnection' não foi configurada.");

        services.AddDbContext<CoreContext>(options => options.UseNpgsql(connectionString));

        services.AddSingleton<NpgsqlDataSource>(_ => NpgsqlDataSource.Create(connectionString));

        services.AddScoped<IVinylRepository, VinylRepository>();

        return services;
    }
}
