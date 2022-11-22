using Base.Application.Exceptions;

namespace TelegramBot.Presentation.Exceptions;

// Used information to user
public class CommandArgumentNotFoundException : NotFoundException
{
    public CommandArgumentNotFoundException(string arg)
        : base($"Arg '{arg}' not found")
    {
    }

    public CommandArgumentNotFoundException()
        : base("Not found arg")
    {
    }
}