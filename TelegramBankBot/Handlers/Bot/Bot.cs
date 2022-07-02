using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

using TelegramBankBot.Logger;

namespace TelegramBankBot.Handlers;

public abstract class Bot
{
    public static ITelegramBotClient BotClient { get; private set; }
    public long Id { get; private set; }
    public Message Message { get; private set; }
    public int MessageId { get; private set; }
    static Bot()
    {
        string token = Program.Configuration.GetSection("Telegram")["Token"];
        BotClient = new TelegramBotClient(token);
    }

    protected static readonly ILogger Log = Program.Log;

    public Bot(Message message)
    {
        Message = message;
        Id = message.Chat.Id;
    }

    public async Task<Message> SendMessageAsync(string text, ParseMode? parseMode = null, IEnumerable<MessageEntity>? entities = null, bool? disableWebPagePreview = null, bool? disableNotification = null, bool? protectContent = null, int? replyToMessageId = null, bool? allowSendingWithoutReply = null, IReplyMarkup? replyMarkup = null, CancellationToken cancellationToken = default)
    {
        Log.Info($"Bot say '{Id}': {text}");
        return await BotClient.SendTextMessageAsync(
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
