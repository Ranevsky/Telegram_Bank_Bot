using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;


namespace TelegramBankBot;

public abstract class Bot
{
    public static ITelegramBotClient BotClient { get; private set; }
    public long Id { get; private set; }
    static Bot()
    {
        string token = Program.Configuration.GetSection("Telegram")["Token"];
        BotClient = new TelegramBotClient(token);
    }

    protected readonly ILogger Log = Program.Log;

    public Bot(long id)
    {
        Id = id;
    }
    public async Task SendMessageAsync(string text, ParseMode? parseMode = null, IEnumerable<MessageEntity>? entities = null, bool? disableWebPagePreview = null, bool? disableNotification = null, bool? protectContent = null, int? replyToMessageId = null, bool? allowSendingWithoutReply = null, IReplyMarkup? replyMarkup = null, CancellationToken cancellationToken = default)
    {
        Log.Info($"Bot say '{Id}': {text}");
        await BotClient.SendTextMessageAsync(
            chatId: Id,
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


    public abstract Task HandleAsync();
}
