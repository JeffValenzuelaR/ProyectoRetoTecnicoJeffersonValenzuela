using EventService.Domain.Enums;
using EventService.Domain.Exceptions;

namespace EventService.Domain.Entities;

public class Event
{
    private readonly List<Zone> _zones = new();

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public DateTimeOffset Date { get; private set; }
    public string Venue { get; private set; } = string.Empty;
    public EventStatus Status { get; private set; } = EventStatus.Draft;
    public DateTimeOffset CreatedAt { get; private set; }

    public IReadOnlyCollection<Zone> Zones => _zones.AsReadOnly();

    private Event()
    {
    }

    private Event(Guid id, string name, DateTimeOffset date, string venue)
    {
        Id = id;
        Name = name;
        Date = date;
        Venue = venue;
        Status = EventStatus.Draft;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public static Event Create(string name, DateTimeOffset date, string venue)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("El nombre del evento es requerido.");
        }

        return new Event(Guid.NewGuid(), name, date, venue);
    }

    public void AddZone(Zone zone)
    {
        ArgumentNullException.ThrowIfNull(zone);
        zone.AssignToEvent(Id);
        _zones.Add(zone);
    }

    public void Publish()
    {
        if (_zones.Count == 0)
        {
            throw new DomainException("No se puede publicar un evento sin al menos una zona.");
        }

        Status = EventStatus.Published;
    }

    public void Cancel()
    {
        Status = EventStatus.Cancelled;
    }
}
