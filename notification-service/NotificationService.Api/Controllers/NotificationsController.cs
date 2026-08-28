using Microsoft.AspNetCore.Mvc;
using NotificationService.Application.Common.Interfaces;

namespace NotificationService.Api.Controllers;

[ApiController]
[Route("notifications")]
public sealed class NotificationsController : ControllerBase
{
    private readonly INotificationJobRepository _notificationJobRepository;

    public NotificationsController(INotificationJobRepository notificationJobRepository)
    {
        _notificationJobRepository = notificationJobRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetLatest([FromQuery] int take = 50, CancellationToken cancellationToken = default)
    {
        var jobs = await _notificationJobRepository.GetLatestAsync(take, cancellationToken);
        return Ok(jobs);
    }
}
