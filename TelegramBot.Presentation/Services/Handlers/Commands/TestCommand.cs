using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Presentation.Models.Args;
using TelegramBot.Presentation.Services.Handlers.CallbackQueries;

namespace TelegramBot.Presentation.Services.Handlers.Commands;

public class TestCommand : Handler<CommandArgs>
{
    public const string CommandName = "TEST";

    private readonly ITelegramBotClient _bot;

    public TestCommand(ITelegramBotClient bot)
    {
        _bot = bot;
    }

    public override async Task HandleAsync(CommandArgs args)
    {
        if (args.GetArg() == CommandName)
        {
            args.ArgsIteration++;
            InlineKeyboardMarkup keyboard = new(InlineKeyboardButton.WithCallbackData("Start", MainCallback.Name));


            await _bot.SendTextMessageAsync(args.ChatId, "Let's go", replyMarkup: keyboard);
        }
        else
        {
            await Successor.HandleAsync(args);
        }
    }
}