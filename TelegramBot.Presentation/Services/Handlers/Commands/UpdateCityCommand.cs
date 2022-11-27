using Bank.Application.Exceptions;
using Bank.Application.Interfaces;
using Microsoft.Extensions.Logging;
using TelegramBot.Application.Interfaces;
using TelegramBot.Presentation.Exceptions;
using TelegramBot.Presentation.Models.Args;

namespace TelegramBot.Presentation.Services.Handlers.Commands;

public class UpdateCityCommand : Handler<CommandArgs>
{
    private const string CommandName = "UPDATE";
    private readonly ILogger<UpdateCityCommand> _logger;
    private readonly IUnitOfWork _uow;
    private readonly IBankUpdater _updater;

    public UpdateCityCommand(IUnitOfWork uow, IBankUpdater updater, ILogger<UpdateCityCommand> logger)
    {
        _uow = uow;
        _updater = updater;
        _logger = logger;
    }

    public override async Task HandleAsync(CommandArgs args)
    {
        if (args.GetArg() == CommandName)
        {
            var iteration = ++args.ArgsIteration;

            if (args.Args.Length - args.ArgsIteration < 1)
            {
                throw new CommandArgumentNotFoundException();
            }

            var cityName = args.GetArg();
            var city = await _uow.Cities.CreateIfNotExistAsync(cityName);

            try
            {
                await _updater.UpdateAsync(city);
            }
            catch (HandlerNotFoundException)
            {
                throw new CommandArgumentNotFoundException(args.Args[iteration]);
            }
            catch (BankUpdateException ex)
            {
                _logger.LogError("Failed to update information due to '{Message}'", ex.Message);
            }
        }
        else
        {
            await Successor.HandleAsync(args);
        }
    }
}