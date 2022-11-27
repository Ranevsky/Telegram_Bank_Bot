using TelegramBot.Application.Interfaces;
using TelegramBot.Presentation.Exceptions;
using TelegramBot.Presentation.Models.Args;

namespace TelegramBot.Presentation.Services.Handlers.Commands;

public class CheckCityCommand : Handler<CommandArgs>
{
    private const string CommandName = "CHECK";
    private readonly IUnitOfWork _uow;

    public CheckCityCommand(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public override async Task HandleAsync(CommandArgs args)
    {
        if (args.GetArg() == CommandName)
        {
            args.ArgsIteration++;
            if (args.Args.Length - args.ArgsIteration < 1)
            {
                throw new CommandArgumentNotFoundException();
            }

            var cityName = args.GetArg();

            await _uow.Banks.CheckLocationAsync(cityName);
        }
        else
        {
            await Successor.HandleAsync(args);
        }
    }
}