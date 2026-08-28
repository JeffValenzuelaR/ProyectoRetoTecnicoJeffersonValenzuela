namespace NotificationService.Application.Common.Interfaces;

public interface IEmailSender
{
    Task SendAsync(string subject, string body, CancellationToken cancellationToken = default);
}
