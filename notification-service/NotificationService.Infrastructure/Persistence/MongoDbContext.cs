using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using NotificationService.Domain.Entities;

namespace NotificationService.Infrastructure.Persistence;

public sealed class MongoDbContext
{
    static MongoDbContext()
    {

        var conventionPack = new ConventionPack
        {
            new CamelCaseElementNameConvention(),
            new EnumRepresentationConvention(BsonType.String),
            new IgnoreExtraElementsConvention(true),
        };
        ConventionRegistry.Register("NotificationService.Infrastructure.Conventions", conventionPack, _ => true);

        BsonSerializer.RegisterSerializer(typeof(Guid), new GuidSerializer(GuidRepresentation.Standard));
    }

    public MongoDbContext(IOptions<MongoDbSettings> options)
    {
        var settings = options.Value;
        var client = new MongoClient(settings.ConnectionString);
        Database = client.GetDatabase(settings.Database);

        NotificationJobs = Database.GetCollection<NotificationJob>("notificationJobs");
    }

    public IMongoDatabase Database { get; }

    public IMongoCollection<NotificationJob> NotificationJobs { get; }

    public async Task EnsureIndexesCreatedAsync(CancellationToken cancellationToken = default)
    {
        var indexKeys = Builders<NotificationJob>.IndexKeys.Ascending(j => j.MessageId);
        var indexModel = new CreateIndexModel<NotificationJob>(
            indexKeys,
            new CreateIndexOptions { Unique = true, Name = "ux_notificationJobs_messageId" });

        await NotificationJobs.Indexes.CreateOneAsync(indexModel, cancellationToken: cancellationToken);
    }
}
