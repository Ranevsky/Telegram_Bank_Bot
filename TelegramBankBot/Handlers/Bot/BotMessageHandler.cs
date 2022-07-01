using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramBankBot.Handlers;

public class BotMessageHandler : Bot
{
    #region ctor's

    public BotMessageHandler(Message message)
        : base(message)
    {

    }

    public BotMessageHandler(Update update)
        : this(update.Message ?? throw new NullReferenceException(nameof(update.Message)))
    {

    }

    #endregion
    public override async Task HandleAsync()
    {
        Handler? handler = Message.Type switch
        {
            MessageType.Text => new MessageTextHandler(this, Message),
            MessageType.Location => new LocationHandler(this, Message.Location!),
            _ => null
        };

        if (handler == null)
        {
            Log.Warning($"'{Message.Type}' is not implemented");
            return;
        }

        await handler.HandleAsync();
    }
}

