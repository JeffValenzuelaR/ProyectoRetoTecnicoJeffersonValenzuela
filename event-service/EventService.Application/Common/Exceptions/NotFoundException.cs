namespace EventService.Application.Common.Exceptions;

public sealed class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }

    public static NotFoundException ForEvent(Guid id) =>
        new($"No se encontro el evento con Id '{id}'.");
}
