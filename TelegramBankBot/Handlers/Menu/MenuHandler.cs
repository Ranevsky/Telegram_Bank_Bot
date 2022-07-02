using TelegramBankBot.Logger;

namespace TelegramBankBot.Handlers.Menu;

public abstract class MenuHandler
{
    private static readonly ILogger Log = Program.Log;
    protected Bot Bot { get; init; }

    public MenuHandler(Bot bot)
    {
        this.Bot = bot;
    }
    public abstract Task HandleAsync();
}
