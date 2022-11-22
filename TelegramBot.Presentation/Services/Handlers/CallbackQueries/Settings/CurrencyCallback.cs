using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Application.Interfaces;
using TelegramBot.Presentation.Models.Args;
using TelegramBot.Presentation.Services.Handlers.CallbackQueries.Settings.Currencies;

namespace TelegramBot.Presentation.Services.Handlers.CallbackQueries.Settings;

public class CurrencyCallback : Handler<CallbackArgs>
{
    public const string Name = "CURR";
    private readonly ITelegramBotClient _bot;

    private readonly Handler<CallbackArgs> _handler;
    private readonly IUnitOfWork _uow;

    public CurrencyCallback(Handler<CallbackArgs> handler, ITelegramBotClient bot, IUnitOfWork uow)
    {
        _handler = handler;
        _bot = bot;
        _uow = uow;
    }

    public static InlineKeyboardMarkup GetKeyboard(IUnitOfWork uow)
    {
        var currNames = uow.TelegramCurrencies.GetAllName();
        InlineKeyboardMarkup markup = new(new[]
        {
            currNames.Select(currName => InlineKeyboardButton.WithCallbackData(currName,
                $"{SettingsCallback.Name}.{Name}.{CurrencySetCallback.Name}.{currName}")).ToArray(),
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Buy",
                    $"{SettingsCallback.Name}.{Name}.{CurrencyOperationCallback.Name}.BUY"),
                InlineKeyboardButton.WithCallbackData("Sell",
                    $"{SettingsCallback.Name}.{Name}.{CurrencyOperationCallback.Name}.SELL")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Back", $"{SettingsCallback.Name}")
            }
        });

        return markup;
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

            await _bot.EditMessageReplyMarkupAsync(args.ChatId, args.MessageId, GetKeyboard(_uow));
        }
        else
        {
            await Successor.HandleAsync(args);
        }
    }
}