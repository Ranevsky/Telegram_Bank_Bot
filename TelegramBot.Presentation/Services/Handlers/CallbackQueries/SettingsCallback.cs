using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Application.Interfaces;
using TelegramBot.Presentation.Models.Args;
using TelegramBot.Presentation.Services.Handlers.CallbackQueries.Settings;

namespace TelegramBot.Presentation.Services.Handlers.CallbackQueries;

public class SettingsCallback : Handler<CallbackArgs>
{
    public const string Name = "SETTINGS";
    private readonly ITelegramBotClient _bot;

    private readonly Handler<CallbackArgs> _handler;
    private readonly IUnitOfWork _uow;

    public SettingsCallback(Handler<CallbackArgs> handler, ITelegramBotClient bot, IUnitOfWork uow)
    {
        _handler = handler;
        _bot = bot;
        _uow = uow;
    }

    public static InlineKeyboardMarkup GetKeyboard()
    {
        InlineKeyboardButton[][] buttons =
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Geolocation",
                    $"{MainCallback.Name}.{Name}.{GeolocationCallback.Name}")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Currency", $"{MainCallback.Name}.{Name}.{CurrencyCallback.Name}")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Back", $"{MainCallback.Name}")
            }
        };

        InlineKeyboardMarkup keyboard = new(buttons);
        return keyboard;
    }

    public override async Task HandleAsync(CallbackArgs args)
    {
        if (args.GetArg() == Name)
        {
            args.ArgsIteration++;

            if (args.Args.Length > args.ArgsIteration)
            {
                await _handler.HandleAsync(args);
                return;
            }

            // If SETTINGS not have args
            var text = await MainCallback.GetTextAsync(_uow, args.From.Id);

            await _bot.EditMessageTextAsync(args.ChatId, args.MessageId, text, replyMarkup: GetKeyboard());
        }
        else
        {
            await Successor.HandleAsync(args);
        }
    }
}