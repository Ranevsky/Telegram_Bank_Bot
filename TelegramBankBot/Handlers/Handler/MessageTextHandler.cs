using Telegram.Bot.Types;

namespace TelegramBankBot.Handlers;

public class MessageTextHandler : Handler
{
    private readonly Message _message = null!;
    public MessageTextHandler(Bot bot, Message message) : base(bot)
    {
        _message = message;
    }
    public override async Task HandleAsync()
    {
        string text = _message.Text!;

        Log.Info($"Message {Bot.Id} {_message.Chat.FirstName} '{text}'");
        if (text.StartsWith('/'))
        {
            Handler cmdHandler = new CommandHandler(Bot, text);

            await cmdHandler.HandleAsync();

            return;
        }

        await Bot.SendMessageAsync(text);
    }
}
