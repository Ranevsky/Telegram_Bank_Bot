using AutoMapper;
using Telegram.Bot.Types;
using TelegramBot.Presentation.Models.Args;

namespace TelegramBot.Presentation.Profiles.Args;

public class CallbackArgsProfile : Profile
{
    public CallbackArgsProfile()
    {
        FromCallbackQuery(this);
    }

    private static void FromCallbackQuery(IProfileExpression profile)
    {
        profile.CreateMap<CallbackQuery, CallbackArgs>()
            .ForMember(args => args.Args, opt =>
            {
                Func<string, string[]> func = args => args.ToUpper().Split('.');

                opt.MapFrom(query => func(query.Data!));
            })
            .ForMember(args => args.CallbackId, opt => { opt.MapFrom(query => query.Id); })
            .ForMember(args => args.KeyboardMarkup, opt => { opt.MapFrom(query => query.Message!.ReplyMarkup); })
            .ForMember(args => args.MessageId, opt => { opt.MapFrom(query => query.Message!.MessageId); })
            .ForMember(args => args.ChatId, opt => { opt.MapFrom(query => query.Message!.Chat); });
    }
}