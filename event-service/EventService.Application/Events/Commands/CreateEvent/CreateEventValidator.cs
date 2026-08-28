using FluentValidation;

namespace EventService.Application.Events.Commands.CreateEvent;

public sealed class CreateEventValidator : AbstractValidator<CreateEventCommand>
{
    public CreateEventValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("El nombre del evento es requerido.");

        RuleFor(x => x.Venue)
            .NotEmpty()
            .WithMessage("El lugar (venue) del evento es requerido.");

        RuleFor(x => x.Zones)
            .NotEmpty()
            .WithMessage("El evento debe tener al menos una zona.");

        RuleForEach(x => x.Zones).ChildRules(zone =>
        {
            zone.RuleFor(z => z.Name)
                .NotEmpty()
                .WithMessage("El nombre de la zona es requerido.");

            zone.RuleFor(z => z.Capacity)
                .GreaterThan(0)
                .WithMessage("El aforo (capacity) de cada zona debe ser mayor a cero.");

            zone.RuleFor(z => z.Price)
                .GreaterThanOrEqualTo(0)
                .WithMessage("El precio de cada zona no puede ser negativo.");
        });
    }
}
