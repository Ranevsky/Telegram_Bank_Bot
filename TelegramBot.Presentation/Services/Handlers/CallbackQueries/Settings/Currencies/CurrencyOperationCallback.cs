using Telegram.Bot;
using TelegramBot.Application.Interfaces;
using TelegramBot.Presentation.Exceptions;
using TelegramBot.Presentation.Models.Args;

namespace TelegramBot.Presentation.Services.Handlers.CallbackQueries.Settings.Currencies;

public class CurrencyOperationCallback : Handler<CallbackArgs>
{
    public const string Name = "OPERATION";

    private readonly ITelegramBotClient _bot;
    private readonly IUnitOfWork _uow;

    public CurrencyOperationCallback(ITelegramBotClient bot, IUnitOfWork uow)
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

            var user = await _uow.Users.GetAsync(args.From.Id, true);

            var operation = args.GetArg();
            args.ArgsIteration++;
            var isChanged = false;

            if (operation == "BUY")
            {
                // Telegram exception (change message)
                if (user.IsBuyOperation != true)
                {
                    isChanged = true;
                    user.IsBuyOperation = true;
                }
            }
            else if (operation == "SELL")
            {
                if (user.IsBuyOperation != false)
                {
                    isChanged = true;
                    user.IsBuyOperation = false;
                }
            }
            else
            {
                throw new HandlerNotFoundException();
            }

            if (isChanged)
            {
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