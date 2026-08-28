using System.Text.Json;
using EventService.Application.Common.Dtos;
using EventService.Application.Common.Interfaces;
using StackExchange.Redis;

namespace EventService.Infrastructure.Caching;

public sealed class RedisEventCacheService : IEventCacheService
{
    private const string AllEventsKey = "events:all";
    private static readonly TimeSpan Ttl = TimeSpan.FromSeconds(30);

    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public RedisEventCacheService(IConnectionMultiplexer connectionMultiplexer)
    {
        _connectionMultiplexer = connectionMultiplexer;
    }

    private IDatabase Database => _connectionMultiplexer.GetDatabase();

    public async Task<IReadOnlyList<EventDto>?> GetAllEventsAsync(CancellationToken cancellationToken)
    {
        var value = await Database.StringGetAsync(AllEventsKey);
        if (value.IsNullOrEmpty)
        {
            return null;
        }

        return JsonSerializer.Deserialize<List<EventDto>>(value!);
    }

    public async Task SetAllEventsAsync(IReadOnlyList<EventDto> events, CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(events);
        await Database.StringSetAsync(AllEventsKey, json, Ttl);
    }

    public async Task InvalidateAllEventsAsync(CancellationToken cancellationToken)
    {
        await Database.KeyDeleteAsync(AllEventsKey);
    }
}
