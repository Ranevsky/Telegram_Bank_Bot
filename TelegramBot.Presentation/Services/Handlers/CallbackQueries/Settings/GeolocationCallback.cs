using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Presentation.Models.Args;
using TelegramBot.Presentation.Services.Handlers.Commands;
using TelegramBot.Presentation.Services.Handlers.Commands.Remove;

namespace TelegramBot.Presentation.Services.Handlers.CallbackQueries.Settings;

public class GeolocationCallback : Handler<CallbackArgs>
{
    public const string Name = "GEO";

    private readonly ITelegramBotClient _bot;

    public GeolocationCallback(ITelegramBotClient bot)
    {
        _bot = bot;
    }

    public override async Task HandleAsync(CallbackArgs args)
    {
        if (args.GetArg() == Name)
        {
            args.ArgsIteration++;

            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
            {
                new[]
                {
                    KeyboardButton.WithRequestLocation("Send location")
                },
                new[]
                {
                    new KeyboardButton($"/{RemoveCommand.CommandName}_{RemoveKeyboard.CommandName}")
                }
            })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = true
            };

            await _bot.SendTextMessageAsync(args.ChatId, "Send your Geo", replyMarkup: replyKeyboardMarkup);
        }
        else
        {
            await Successor.HandleAsync(args);
        }
    }
}