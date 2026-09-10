using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using TheBand.AuthDomain.Interfaces;
using TheBand.AuthInfrastructure.Repositories;

namespace TheBand.AuthInfrastructure.Data;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {       
        MongoMappings.Configure();

        var connectionString = configuration["MongoDB:ConnectionString"]
                               ?? "mongodb://localhost:27017";

        services.AddSingleton<IMongoClient>(new MongoClient(connectionString));

        services.AddScoped<AuthContext>();

       services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}
