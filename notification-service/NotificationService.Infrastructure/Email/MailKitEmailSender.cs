using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using NotificationService.Application.Common.Interfaces;
using Polly;
using Polly.Retry;

namespace NotificationService.Infrastructure.Email;

public sealed class MailKitEmailSender : IEmailSender
{
    private const string FromAddress = "notificaciones@plataforma-eventos.local";
    private const string ToAddress = "operaciones@plataforma-eventos.local";

    private readonly SmtpSettings _settings;
    private readonly ILogger<MailKitEmailSender> _logger;
    private readonly ResiliencePipeline _resiliencePipeline;

    public MailKitEmailSender(IOptions<SmtpSettings> settings, ILogger<MailKitEmailSender> logger)
    {
        _settings = settings.Value;
        _logger = logger;

        _resiliencePipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = 2,
                BackoffType = DelayBackoffType.Constant,
                Delay = TimeSpan.FromMilliseconds(500),
                OnRetry = args =>
                {
                    _logger.LogWarning(
                        args.Outcome.Exception,
                        "Reintento {AttemptNumber} enviando correo via SMTP {Host}:{Port}",
                        args.AttemptNumber + 1,
                        _settings.Host,
                        _settings.Port);
                    return ValueTask.CompletedTask;
                },
            })
            .Build();
    }

    public async Task SendAsync(string subject, string body, CancellationToken cancellationToken = default)
    {
        await _resiliencePipeline.ExecuteAsync(
            async ct =>
            {
                var message = new MimeMessage();
                message.From.Add(MailboxAddress.Parse(FromAddress));
                message.To.Add(MailboxAddress.Parse(ToAddress));
                message.Subject = subject;
                message.Body = new TextPart("plain") { Text = body };

                using var client = new SmtpClient();
                await client.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.None, ct);
                await client.SendAsync(message, ct);
                await client.DisconnectAsync(true, ct);
            },
            cancellationToken);
    }
}
