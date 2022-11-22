using TelegramBot.Presentation.Exceptions;

namespace TelegramBot.Presentation.Services.Handlers;

public abstract class Handler<TValue>
{
    private Handler<TValue>? _successor;

    public Handler<TValue> Successor
    {
        get => _successor ?? throw new HandlerNotFoundException();
        set => _successor = value;
    }

    public abstract Task HandleAsync(TValue value);
}