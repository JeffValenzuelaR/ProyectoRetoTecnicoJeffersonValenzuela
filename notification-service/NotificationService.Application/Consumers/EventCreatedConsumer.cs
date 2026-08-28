using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using MassTransit;
using Microsoft.Extensions.Logging;
using NotificationService.Application.Common.Interfaces;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Enums;
using Shared.Contracts;

namespace NotificationService.Application.Consumers;

public sealed class EventCreatedConsumer : IConsumer<EventCreated>
{
    private readonly INotificationJobRepository _notificationJobRepository;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<EventCreatedConsumer> _logger;

    public EventCreatedConsumer(
        INotificationJobRepository notificationJobRepository,
        IEmailSender emailSender,
        ILogger<EventCreatedConsumer> logger)
    {
        _notificationJobRepository = notificationJobRepository;
        _emailSender = emailSender;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<EventCreated> context)
    {
        var message = context.Message;
        var cancellationToken = context.CancellationToken;

        var job = await _notificationJobRepository.FindByMessageIdAsync(message.MessageId, cancellationToken);

        if (job is { Status: NotificationStatus.Sent })
        {
            _logger.LogInformation(
                "Mensaje ya procesado con exito, idempotencia OK. MessageId={MessageId} EventId={EventId}",
                message.MessageId,
                message.EventId);
            return;
        }

        if (job is null)
        {
            job = new NotificationJob
            {
                MessageId = message.MessageId,
                EventId = message.EventId,
                EventName = message.Name,
                OccurredAt = message.OccurredAt,
                CorrelationId = message.CorrelationId,
                PayloadHash = ComputePayloadHash(message),
                Status = NotificationStatus.Processing,
            };

            var inserted = await _notificationJobRepository.TryInsertAsync(job, cancellationToken);
            if (!inserted)
            {

                job = await _notificationJobRepository.FindByMessageIdAsync(message.MessageId, cancellationToken);
                if (job is { Status: NotificationStatus.Sent })
                {
                    _logger.LogInformation(
                        "Carrera de idempotencia resuelta: otra instancia ya lo envio. MessageId={MessageId}",
                        message.MessageId);
                    return;
                }
            }
        }

        try
        {
            var subject = $"Nuevo evento creado: {message.Name}";
            var body =
                $"Se creo el evento '{message.Name}'." + Environment.NewLine +
                $"Fecha: {message.OccurredAt:u}" + Environment.NewLine +
                $"EventId: {message.EventId}" + Environment.NewLine +
                $"CorrelationId: {message.CorrelationId}";

            await _emailSender.SendAsync(subject, body, cancellationToken);

            job!.Status = NotificationStatus.Sent;
            job.ErrorMessage = null;
            job.UpdatedAt = DateTimeOffset.UtcNow;
            await _notificationJobRepository.UpdateAsync(job, cancellationToken);
        }
        catch (Exception ex)
        {
            job!.Status = NotificationStatus.Failed;
            job.ErrorMessage = ex.Message;
            job.UpdatedAt = DateTimeOffset.UtcNow;
            await _notificationJobRepository.UpdateAsync(job, cancellationToken);

            _logger.LogError(
                ex,
                "Fallo el envio de correo para EventId={EventId} MessageId={MessageId}. Se relanza para que MassTransit aplique su politica de reintentos.",
                message.EventId,
                message.MessageId);

            throw;
        }
    }

    private static string ComputePayloadHash(EventCreated message)
    {
        var json = JsonSerializer.Serialize(message);
        var bytes = Encoding.UTF8.GetBytes(json);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
