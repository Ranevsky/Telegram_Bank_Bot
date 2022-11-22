using AutoMapper;
using Telegram.Bot.Types;
using TelegramBot.Presentation.Models.Args;

namespace TelegramBot.Presentation.Profiles.Args;

public class TextArgsProfile : Profile
{
    public TextArgsProfile()
    {
        FromMessage(this);
    }

    private static void FromMessage(IProfileExpression profile)
    {
        profile.CreateMap<Message, TextArgs>();
    }
}