using Telegram.Bot;
using TelegramBot.Application.Interfaces;
using TelegramBot.Presentation.Exceptions;
using TelegramBot.Presentation.Models.Args;

namespace TelegramBot.Presentation.Services.Handlers.CallbackQueries.Settings.Currencies;

public class CurrencySetCallback : Handler<CallbackArgs>
{
    public const string Name = "CURRSET";
    private readonly ITelegramBotClient _bot;

    private readonly IUnitOfWork _uow;

    public CurrencySetCallback(ITelegramBotClient bot, IUnitOfWork uow)
    {
        _bot = bot;
        _uow = uow;
    }

    public override async Task HandleAsync(CallbackArgs args)
    {
        if (args.GetArg() == Name)
        {
            args.ArgsIteration++;

            if (args.Args.Length <= args.ArgsIteration)
            {
                throw new HandlerNotFoundException();
            }

            var currName = args.GetArg();
            var curr = await _uow.TelegramCurrencies.GetAsync(currName);

            var user = await _uow.Users.GetWithCurrencyAsync(args.From.Id, true);

            if (user.SelectedCurrency?.Name != curr.Name)
            {
                user.SelectedCurrency = curr;

                await _uow.SaveAsync();

                var text = await MainCallback.GetTextAsync(_uow, args.From.Id);
                await _bot.EditMessageTextAsync(args.ChatId, args.MessageId, text,
                    replyMarkup: CurrencyCallback.GetKeyboard(_uow));
            }
        }
        else
        {
            await Successor.HandleAsync(args);
        }
    }
}