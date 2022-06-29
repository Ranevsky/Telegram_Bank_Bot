
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

using TelegramBankBot.Handlers;

namespace TelegramBankBot;

public class BotMessageHandler : Bot
{
    protected readonly Message _message = null!;

    #region ctor's

    public BotMessageHandler(Message message)
        : base(message.Chat.Id)
    {
        _message = message ?? throw new NullReferenceException(nameof(message));
    }

    public BotMessageHandler(Update update)
        : this(update.Message ?? throw new NullReferenceException(nameof(update.Message)))
    {
        
    }

    #endregion
    public override async Task HandleAsync()
    {
        switch (_message.Type)
        {
            case MessageType.Text:
            {
                await HandleMessageTextAsync();
                break;
            }

            case MessageType.Dice:
            {
                await SendMessageAsync(_message.Dice!.Value.ToString());
                break;
            }

            case MessageType.Location:
            {
                await LocationHandler.HandleAsync(this, _message.Location!);
                break;
            }

            default:
            {
                Log.Warning($"'{_message.Type}' is not implemented");
                return;
            }
        }
    }
    private async Task HandleMessageTextAsync()
    {
        string text = _message.Text!;

        Log.Info($"Message {Id} {_message.Chat.FirstName} '{text}'");
        if (text.StartsWith('/'))
        {
            var cmdHandler = new CommandHandler(this);

            await cmdHandler.HandleAsync(text);

            return;
        }

        await SendMessageAsync(text);
    }
}

