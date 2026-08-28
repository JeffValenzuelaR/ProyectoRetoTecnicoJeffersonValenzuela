using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using MongoDB.Driver;
using NotificationService.Infrastructure.Persistence;

namespace NotificationService.Api.HealthChecks;

public sealed class MongoDbHealthCheck : IHealthCheck
{
    private readonly MongoDbContext _context;

    public MongoDbHealthCheck(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.Database.RunCommandAsync((Command<BsonDocument>)"{ ping: 1 }", cancellationToken: cancellationToken);
            return HealthCheckResult.Healthy("MongoDB respondio al ping.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("No se pudo contactar a MongoDB.", ex);
        }
    }
}
