using AutoMapper;

namespace TelegramBankBot.Automapper;

public static class AutoMapping
{
    private static readonly IMapper _mapper;

    static AutoMapping()
    {
        MapperConfiguration? config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Telegram.Bot.Types.User, TelegramBankBot.Model.User>()
                .ForMember(dbUser => dbUser.Name, opt =>
                {
                    opt.MapFrom(tgUser => tgUser.FirstName);
                })
                .ForMember(dbUser => dbUser.Id, opt =>
                {
                    opt.MapFrom(tgUser => tgUser.Id);
                });

            cfg.CreateMap<Telegram.Bot.Types.Location, TelegramBankBot.Model.Location>()
                .ForMember(userLocation => userLocation.Latitude, opt =>
                {
                    opt.MapFrom(tgLocation => tgLocation.Latitude);
                })
                .ForMember(userLocation => userLocation.Longitude, opt =>
                {
                    opt.MapFrom(tgLocation => tgLocation.Longitude);
                });
        });
        _mapper = config.CreateMapper();
    }

    public static IMapper Mapper => _mapper;
}
