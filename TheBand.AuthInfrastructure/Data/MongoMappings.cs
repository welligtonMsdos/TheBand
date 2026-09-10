using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using TheBand.AuthDomain.Entities;
using TheBand.AuthDomain.Enum;

namespace TheBand.AuthInfrastructure.Data;

public static class MongoMappings
{
    public static void Configure()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(User)))
        {
            BsonClassMap.RegisterClassMap<User>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(c => c._id)
                  .SetIdGenerator(StringObjectIdGenerator.Instance)
                  .SetSerializer(new StringSerializer(BsonType.ObjectId));

                cm.MapMember(c => c.Role)
                  .SetElementName("Role")
                  .SetSerializer(new EnumSerializer<UserRole>(BsonType.String));
            });
        }
    }
}
