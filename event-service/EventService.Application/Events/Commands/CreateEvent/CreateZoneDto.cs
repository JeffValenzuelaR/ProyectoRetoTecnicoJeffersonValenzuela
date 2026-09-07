namespace EventService.Application.Events.Commands.CreateEvent;

public sealed class CreateZoneDto
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Capacity { get; set; }
}
