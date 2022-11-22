using Bank.Application.Models;
using TelegramBot.Application.Interfaces;
using TelegramBot.Presentation.Models.Args;

namespace TelegramBot.Presentation.Services.Handlers.CallbackQueries.Bank;

public class BestCurrencyExchange : Handler<CallbackArgs>
{
    public const string Name = "BESTEXCHANGE";
    private readonly GetExchange _getExchange;

    private readonly IUnitOfWork _uow;

    public BestCurrencyExchange(IUnitOfWork uow, GetExchange getExchange)
    {
        _uow = uow;
        _getExchange = getExchange;
    }

    public override async Task HandleAsync(CallbackArgs args)
    {
        if (args.Args.Length - args.ArgsIteration >= 2 && args.GetArg() == Name)
        {
            const int take = 10;
            var page = int.Parse(args.Args[args.ArgsIteration + 1]);
            args.ArgsIteration += 2;
            var isBuyOperation = _uow.Users.GetBuyOperation(args.From.Id) ?? false;

            IOrderedEnumerable<DepartmentByDistance> Order(IEnumerable<DepartmentByDistance> model)
            {
                var basicOrder = isBuyOperation
                    ? model.OrderBy(x => x.Currency.Buy)
                    : model.OrderByDescending(x => x.Currency.Sell);

                return basicOrder.ThenBy(x => x.Distance);
            }

            await _getExchange.GetFormsAsync(args, Order, Name, page, take);
        }
        else
        {
            await Successor.HandleAsync(args);
        }
    }
}