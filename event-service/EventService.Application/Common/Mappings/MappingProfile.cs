using AutoMapper;
using EventService.Application.Common.Dtos;
using EventService.Domain.Entities;

namespace EventService.Application.Common.Mappings;

public sealed class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Event, EventDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Zones, opt => opt.MapFrom(src => src.Zones));

        CreateMap<Zone, ZoneDto>();
    }
}
