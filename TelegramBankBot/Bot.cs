using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;


namespace TelegramBankBot;

public class Bot
{
    public static ITelegramBotClient BotClient { get; private set; }
    public long Id { get; private set; }
    static Bot()
    {
        string token = Program.Configuration.GetSection("Telegram")["Token"];
        BotClient = new TelegramBotClient(token);
    }

    protected readonly ILogger Log = new ConsoleLogger();

    public Bot(long id)
    {
        Id = id;
    }
    protected async Task SendMessageAsync(string text, ParseMode? parseMode = null, IEnumerable<MessageEntity>? entities = null, bool? disableWebPagePreview = null, bool? disableNotification = null, bool? protectContent = null, int? replyToMessageId = null, bool? allowSendingWithoutReply = null, IReplyMarkup? replyMarkup = null, CancellationToken cancellationToken = default)
    {
        Console.WriteLine(Id);
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

}
