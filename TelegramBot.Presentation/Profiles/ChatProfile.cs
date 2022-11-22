using AutoMapper;
using Telegram.Bot.Types;

namespace TelegramBot.Presentation.Profiles;

public class ChatProfile : Profile
{
    public ChatProfile()
    {
        ToLong(this);
    }

    private static void ToLong(IProfileExpression profile)
    {
        profile.CreateMap<Chat, long>()
            .ConvertUsing(chat => chat.Id);
    }
}