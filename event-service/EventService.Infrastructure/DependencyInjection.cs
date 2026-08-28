using EventService.Application.Common.Interfaces;
using EventService.Infrastructure.Caching;
using EventService.Infrastructure.Messaging;
using EventService.Infrastructure.Persistence;
using EventService.Infrastructure.Persistence.Repositories;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace EventService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var postgresConnectionString = configuration.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException("Falta la connection string 'Postgres'.");

        services.AddDbContext<EventDbContext>(options =>
            options.UseNpgsql(postgresConnectionString));

        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IEventCreatedPublisher, EventCreatedPublisher>();

        var redisConnectionString = configuration["Redis:Connection"]
            ?? throw new InvalidOperationException("Falta la configuracion 'Redis:Connection'.");

        services.AddSingleton<IConnectionMultiplexer>(
            _ => ConnectionMultiplexer.Connect(redisConnectionString));
        services.AddScoped<IEventCacheService, RedisEventCacheService>();

        var rabbitMqHost = configuration["RabbitMq:Host"] ?? "localhost";
        var rabbitMqUsername = configuration["RabbitMq:Username"] ?? "guest";
        var rabbitMqPassword = configuration["RabbitMq:Password"] ?? "guest";

        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();

            x.AddEntityFrameworkOutbox<EventDbContext>(o =>
            {
                o.UsePostgres();
                o.UseBusOutbox();
            });

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbitMqHost, "/", h =>
                {
                    h.Username(rabbitMqUsername);
                    h.Password(rabbitMqPassword);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        services.AddHealthChecks()
            .AddNpgSql(postgresConnectionString, name: "postgres")
            .AddRedis(redisConnectionString, name: "redis");

        return services;
    }
}
