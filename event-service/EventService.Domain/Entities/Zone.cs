using EventService.Domain.Exceptions;

namespace EventService.Domain.Entities;

public class Zone
{
    public Guid Id { get; private set; }
    public Guid EventId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int Capacity { get; private set; }

    private Zone()
    {
    }

    private Zone(Guid id, Guid eventId, string name, decimal price, int capacity)
    {
        Id = id;
        EventId = eventId;
        Name = name;
        Price = price;
        Capacity = capacity;
    }

    public static Zone Create(string name, decimal price, int capacity, Guid? eventId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("El nombre de la zona es requerido.");
        }

        if (price < 0)
        {
            throw new DomainException("El precio de la zona no puede ser negativo.");
        }

        if (capacity <= 0)
        {
            throw new DomainException("El aforo (capacity) de la zona debe ser mayor a cero.");
        }

        return new Zone(Guid.NewGuid(), eventId ?? Guid.Empty, name, price, capacity);
    }

    internal void AssignToEvent(Guid eventId)
    {
        EventId = eventId;
    }
}
