using AutoMapper;
using Telegram.Bot.Types;

namespace TelegramBot.Presentation.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        FromUserTg(this);
    }

    private static void FromUserTg(IProfileExpression profile)
    {
        profile.CreateMap<User, Domain.Entities.User>()
            .ForMember(dbUser => dbUser.FirstName, opt => { opt.MapFrom(tgUser => tgUser.FirstName); })
            .ForMember(dbUser => dbUser.Id, opt => { opt.MapFrom(tgUser => tgUser.Id); })
            .ForMember(dbUser => dbUser.Id, opt => { opt.MapFrom(tgUser => tgUser.Id); });
    }
}