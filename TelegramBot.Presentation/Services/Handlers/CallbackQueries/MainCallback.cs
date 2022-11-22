using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Application.Interfaces;
using TelegramBot.Presentation.Models.Args;
using TelegramBot.Presentation.Services.Handlers.CallbackQueries.Bank;

namespace TelegramBot.Presentation.Services.Handlers.CallbackQueries;

public class MainCallback : Handler<CallbackArgs>
{
    public const string Name = "MAIN";
    private readonly ITelegramBotClient _bot;

    private readonly Handler<CallbackArgs> _handler;
    private readonly IUnitOfWork _uow;

    public MainCallback(
        Handler<CallbackArgs> handler,
        ITelegramBotClient bot,
        IUnitOfWork uow)
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
                InlineKeyboardButton.WithCallbackData("Best exchange",
                    $"{BankCallback.Name}.{BestCurrencyExchange.Name}.0"),
                InlineKeyboardButton.WithCallbackData("Near banks", $"{BankCallback.Name}.{NearBankCallback.Name}.0")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Settings", $"{Name}.{SettingsCallback.Name}")
            }
        };

        InlineKeyboardMarkup keyboard = new(buttons);
        return keyboard;
    }

    public static async Task<string> GetTextAsync(IUnitOfWork uow, long id)
    {
        var user = await uow.Users.GetWithLocationAndCurrencyAsync(id);

        var location = user.Location;
        var textLocation = location is null
            ? "Location: unknown"
            : $"Location: {location.Latitude} {location.Longitude}";


        var operationCurrency = user.IsBuyOperation is null
            ? "unknown"
            : user.IsBuyOperation == true
                ? "Buy"
                : "Sell";

        var selectedCurrency = $"Selected curr: {user.SelectedCurrency?.Name ?? "unknown"} {operationCurrency}";

        var text = $"{textLocation}\n{selectedCurrency}";

        return text;
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

            var keyBoard = GetKeyboard();

            var text = await GetTextAsync(_uow, args.From.Id);

            await _bot.EditMessageTextAsync(args.ChatId, args.MessageId, text, replyMarkup: keyBoard);
        }
        else
        {
            await Successor.HandleAsync(args);
        }
    }
}