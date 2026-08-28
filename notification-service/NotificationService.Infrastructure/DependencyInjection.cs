using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Application.Common.Interfaces;
using NotificationService.Infrastructure.Email;
using NotificationService.Infrastructure.Messaging;
using NotificationService.Infrastructure.Persistence;
using NotificationService.Infrastructure.Persistence.Repositories;

namespace NotificationService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoDbSettings>(configuration.GetSection("MongoDb"));
        services.Configure<SmtpSettings>(configuration.GetSection("Smtp"));

        services.AddSingleton<MongoDbContext>();

        services.AddScoped<INotificationJobRepository, NotificationJobRepository>();
        services.AddScoped<IEmailSender, MailKitEmailSender>();

        services.AddMessaging(configuration);

        return services;
    }
}
