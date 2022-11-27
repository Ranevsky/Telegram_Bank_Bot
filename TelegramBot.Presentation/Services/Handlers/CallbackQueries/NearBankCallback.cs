using Bank.Application.Models;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Application.Interfaces;
using TelegramBot.Presentation.Models.Args;

namespace TelegramBot.Presentation.Services.Handlers.CallbackQueries;

public class NearBankCallback : Handler<CallbackArgs>
{
    public const string Name = "NEARBANKS";
    private readonly GetExchange _getExchange;

    private readonly IUnitOfWork _uow;

    public NearBankCallback(IUnitOfWork uow, GetExchange getExchange)
    {
        _uow = uow;
        _getExchange = getExchange;
    }

    public static InlineKeyboardMarkup GetKeyboard()
    {
        InlineKeyboardButton[][] buttons =
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Near banks", $"{Name}.{Name}")
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
        if (args.Args.Length - args.ArgsIteration >= 2 && args.GetArg() == Name)
        {
            const int take = 10;
            var page = int.Parse(args.Args[args.ArgsIteration + 1]);
            args.ArgsIteration += 2;
            var isBuyOperation = await _uow.Users.GetBuyOperationAsync(args.From.Id) ?? false;

            IOrderedEnumerable<DepartmentByDistance> Order(IEnumerable<DepartmentByDistance> model)
            {
                var basicOrder = model.OrderBy(x => x.Distance);

                return isBuyOperation
                    ? basicOrder.ThenBy(x => x.CurrencyExchange.Buy)
                    : basicOrder.ThenByDescending(x => x.CurrencyExchange.Sell);
            }

            await _getExchange.GetFormsAsync(args, Order, Name, page, take);
        }
        else
        {
            await Successor.HandleAsync(args);
        }
    }
}