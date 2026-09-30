using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using TheBand.AuthDomain.Entities;

namespace TheBand.AuthInfrastructure.Data;

public class AuthContext
{
    private readonly IMongoDatabase _database;

    public IMongoCollection<User> Users => _database.GetCollection<User>("User");

    public AuthContext(IMongoClient client,
                       IConfiguration config)
    {
        var dbName = config["MongoDB:DatabaseName"] ?? "TomAuth";

        _database = client.GetDatabase(dbName);
    }
}
