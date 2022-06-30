namespace TelegramBankBot;

public abstract class Handler
{
    protected Bot Bot { get; private set; }
    protected ILogger Log { get; private set; }
    public Handler(Bot bot)
    {
        Bot = bot;
        Log = Program.Log;
    }
    public abstract Task HandleAsync();
}
