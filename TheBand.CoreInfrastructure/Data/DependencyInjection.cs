using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
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

        services.Scan(scan => scan
                .FromAssemblyOf<VinylRepository>()
                .AddClasses(c => c.Where(t => t.Name.EndsWith("Repository")))
                .AsMatchingInterface()
                .WithScopedLifetime());

        return services;
    }
}
