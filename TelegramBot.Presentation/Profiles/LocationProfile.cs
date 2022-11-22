using AutoMapper;
using Telegram.Bot.Types;

namespace TelegramBot.Presentation.Profiles;

public class LocationProfile : Profile
{
    public LocationProfile()
    {
        FromTelegramLocation(this);
    }

    private static void FromTelegramLocation(IProfileExpression profile)
    {
        profile.CreateMap<Location, Base.Domain.Entities.Location>()
            .ForMember(userLocation => userLocation.Latitude,
                opt => { opt.MapFrom(tgLocation => tgLocation.Latitude); })
            .ForMember(userLocation => userLocation.Longitude,
                opt => { opt.MapFrom(tgLocation => tgLocation.Longitude); })
            .ForMember(userLocation => userLocation.Id, opt => { opt.MapFrom(tgUser => 0); })
            .ReverseMap();
    }
}