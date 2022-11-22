using TelegramBot.Presentation.Exceptions;
using TelegramBot.Presentation.Models.Args;

namespace TelegramBot.Presentation.Services.Handlers.Commands;

public class RemoveCommand : Handler<CommandArgs>
{
    public const string CommandName = "REMOVE";

    private readonly Handler<CommandArgs> _handler;

    public RemoveCommand(Handler<CommandArgs> handler)
    {
        _handler = handler;
    }

    public override async Task HandleAsync(CommandArgs commandArgs)
    {
        if (commandArgs.Args[0] == CommandName)
        {
            if (commandArgs.Args.Length <= 1)
            {
                throw new CommandArgumentNotFoundException();
            }

            try
            {
                await _handler.HandleAsync(commandArgs);
            }
            catch (HandlerNotFoundException)
            {
                throw new CommandArgumentNotFoundException(commandArgs.Args[1]);
            }
        }
        else
        {
            await Successor.HandleAsync(commandArgs);
        }
    }
}