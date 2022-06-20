
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramBankBot;

public class BotHandleMessage : Bot
{
    protected readonly Message _message = null!;

    #region ctor's

    public BotHandleMessage(ITelegramBotClient botClient, Message message)
        : base(botClient, message.Chat.Id)
    {
        _message = message ?? throw new NullReferenceException(nameof(message));
    }

    public BotHandleMessage(ITelegramBotClient botClient, Update update)
        : this(
              botClient,
              update.Message ?? throw new NullReferenceException(nameof(update.Message)))
    {
        
    }

    #endregion
    public async Task HandleMessageAsync()
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

            default:
            {
                _log.Warning($"'{_message.Type}' is not implemented");
                return;
            }
        }

    }
    private async Task HandleMessageTextAsync()
    {
        string text = _message.Text!;

        _log.Info($"Message {_id} {_message.Chat.FirstName} '{text}'");
        if (text.StartsWith('/'))
        {
            await HandleCommandTextAsync(text);
            return;
        }

        await SendMessageAsync(text);
    }

    private int sum = 0;
    private async Task HandleCommandTextAsync(string commandString)
    {
        string[] commandArgs = commandString.GetStrings();
        string command = commandArgs[0];

        switch (command.ToUpper())
        {
            case "/HELLO":
                HelloCommand();
                break;

            case "/SUM":
                await SumCommand();
                break;

            default:
                _log.Warning($"Command '{commandString}' not implemented");
                return;
        }

        async Task SumCommand()
        {
            Console.WriteLine(commandArgs.Length);

            if (!(commandArgs.Length > 1))
            {
                await SendMessageAsync("Нет 2-ого аргумента (цифры)");
                return;
            }

            if (!int.TryParse(commandArgs[1], out int num))
            {
                await SendMessageAsync("2-ой аргумент должен быть цифрой");
                return;
            }

            sum += num;
            await SendMessageAsync(sum.ToString());
        }
        void HelloCommand()
        {
            _log.Info("Hello");
        }
    }
}