using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Application.Consumers;

namespace NotificationService.Infrastructure.Messaging;

public static class DependencyInjection
{

    public const string NotificationEventCreatedQueue = "notification-event-created";

    public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitMqSettings = configuration.GetSection("RabbitMq").Get<RabbitMqSettings>() ?? new RabbitMqSettings();

        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.AddConsumer<EventCreatedConsumer>();

            busConfigurator.UsingRabbitMq((context, rabbitCfg) =>
            {
                rabbitCfg.Host(rabbitMqSettings.Host, "/", hostCfg =>
                {
                    hostCfg.Username(rabbitMqSettings.Username);
                    hostCfg.Password(rabbitMqSettings.Password);
                });

                rabbitCfg.ReceiveEndpoint(NotificationEventCreatedQueue, endpointCfg =>
                {

                    endpointCfg.UseMessageRetry(retryCfg => retryCfg.Exponential(
                        3,
                        TimeSpan.FromSeconds(2),
                        TimeSpan.FromSeconds(30),
                        TimeSpan.FromSeconds(2)));

                    endpointCfg.ConfigureConsumer<EventCreatedConsumer>(context);
                });
            });
        });

        return services;
    }
}
