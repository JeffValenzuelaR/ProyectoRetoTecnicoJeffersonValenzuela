namespace NotificationService.Infrastructure.Persistence;

public sealed class MongoDbSettings
{
    public string ConnectionString { get; set; } = string.Empty;

    public string Database { get; set; } = string.Empty;
}
