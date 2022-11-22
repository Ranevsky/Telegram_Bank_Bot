using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Presentation.Models.Args;

namespace TelegramBot.Presentation.Services.Handlers.Commands.Remove;

public class RemoveKeyboard : Handler<CommandArgs>
{
    public const string CommandName = "KEYBOARD";

    private readonly ITelegramBotClient _bot;

    public RemoveKeyboard(ITelegramBotClient bot)
    {
        _bot = bot;
    }

    /// <param name="bot">Telegram bot</param>
    /// <param name="text">If the value is null, then the default value will be "<b>Keyboard removed</b>"</param>
    /// <param name="chatId">Chat id in telegram</param>
    /// <param name="disableNotification">Notification in chat</param>
    /// <param name="delay">Delay before deleting</param>
    public static async Task RemoveKeyboardAsync(
        ITelegramBotClient bot,
        string? text,
        long chatId,
        bool? disableNotification = true,
        int? delay = 2000)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            text = "Keyboard removed";
        }

        var taskMessage = bot.SendTextMessageAsync(
            chatId,
            text,
            replyMarkup: new ReplyKeyboardRemove(),
            disableNotification: disableNotification);

        var msg = await taskMessage;

        if (delay is not null)
        {
            taskMessage.Wait();
            await Task.Delay(delay.Value);
        }

        await bot.DeleteMessageAsync(
            chatId,
            msg.MessageId);
    }

    public override async Task HandleAsync(CommandArgs args)
    {
        if (args.Args[1] == CommandName)
        {
            await RemoveKeyboardAsync(_bot, null, args.ChatId);
        }
        else
        {
            await Successor.HandleAsync(args);
        }
    }
}