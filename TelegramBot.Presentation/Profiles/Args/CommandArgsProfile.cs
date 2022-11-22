using AutoMapper;
using TelegramBot.Presentation.Models.Args;

namespace TelegramBot.Presentation.Profiles.Args;

public class CommandArgsProfile : Profile
{
    public CommandArgsProfile()
    {
        FromTextArgs(this);
    }

    private static void FromTextArgs(IProfileExpression profile)
    {
        profile.CreateMap<TextArgs, CommandArgs>()
            .ForMember(c => c.Args, opt =>
            {
                Func<string, string[]> func = args => args[1..]
                    .ToUpper()
                    .Split('_');

                opt.MapFrom(t => func(t.Text));
            });
    }
}