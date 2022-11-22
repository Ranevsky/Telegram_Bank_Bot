using TelegramBot.Presentation.Models.Args;

namespace TelegramBot.Presentation.Services.Handlers.CallbackQueries;

public class BankCallback : Handler<CallbackArgs>
{
    public const string Name = "BANK";

    private readonly Handler<CallbackArgs> _handler;

    public BankCallback(Handler<CallbackArgs> handler)
    {
        _handler = handler;
    }

    public override async Task HandleAsync(CallbackArgs args)
    {
        if (args.GetArg() == Name)
        {
            args.ArgsIteration++;
            await _handler.HandleAsync(args);
        }
        else
        {
            await Successor.HandleAsync(args);
        }
    }
}