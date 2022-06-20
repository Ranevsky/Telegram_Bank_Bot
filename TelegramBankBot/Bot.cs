using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;


namespace TelegramBankBot;

public class Bot
{
    protected readonly ITelegramBotClient _bot = null!;
    protected readonly long _id;

    protected readonly ILogger _log = new ConsoleLogger();

    public Bot(ITelegramBotClient botClient, long id)
    {
        _bot = botClient ?? throw new NullReferenceException(nameof(botClient));
        _id = id;
    }
    protected async Task SendMessageAsync(string text, ParseMode? parseMode = null, IEnumerable<MessageEntity>? entities = null, bool? disableWebPagePreview = null, bool? disableNotification = null, bool? protectContent = null, int? replyToMessageId = null, bool? allowSendingWithoutReply = null, IReplyMarkup? replyMarkup = null, CancellationToken cancellationToken = default)
    {
        Console.WriteLine(_id);
        await _bot.SendTextMessageAsync(
            chatId: _id,
            text: text,
            parseMode: parseMode,
            entities: entities,
            disableWebPagePreview: disableWebPagePreview,
            disableNotification: disableNotification,
            protectContent: protectContent,
            replyToMessageId: replyToMessageId,
            allowSendingWithoutReply: allowSendingWithoutReply,
            replyMarkup: replyMarkup,
            cancellationToken: cancellationToken
            );
    }

}
