using Telegram.Bot.Types;

using TelegramBankBot.Menu;

namespace TelegramBankBot.Handlers;

public class BotCallbackHandler : Bot
{
    private readonly CallbackQuery _callback = null!;
    #region ctor's
    public BotCallbackHandler(CallbackQuery callbackQuery)
        : base(callbackQuery.Message ?? throw new NullReferenceException(nameof(callbackQuery)))
    {
        _callback = callbackQuery;

        InitializeDictionary();
    }


    private void InitializeDictionary()
    {
        dict = new()
        {
#warning retunr instance and use abstraction (instance).HandleAsync();
            { nameof(MainMenu), async (args, messageId) => { await new MainMenu(this).HandleAsync(args, messageId); } }
        };
    }

    public BotCallbackHandler(Update update)
        : this(update.CallbackQuery ?? throw new NullReferenceException(nameof(update.CallbackQuery)))
    {

    }
    #endregion

    private Dictionary<string, Func<string[], int, Task>> dict = null!;
    public override async Task HandleAsync()
    {
        Message msg = _callback.Message!;
        // Data -> Id кнопки?
        // Text -> Текст сообщения
        // MessageId -> Id сообщения
        string text = $"{msg.Text} {msg.MessageId} {_callback.Data}";
        Log.Info(text);


        string[] args = _callback.Data!.Split('.');
        try
        {
            if (dict.TryGetValue(args[0], out Func<string[], int, Task>? act))
            {
                await act.Invoke(args, msg.MessageId);
            }
            else
            {
                throw new Exception($"'{_callback.Data!}' not found");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
        }
        //async Task SetLocation()
        //{
        //    InlineKeyboardMarkup inline = new(new[]
        //    {
        //        new InlineKeyboardButton[]
        //        {
        //            InlineKeyboardButton.WithCallbackData("Set Location")
        //        },
        //        new InlineKeyboardButton[]
        //        {
        //            InlineKeyboardButton.WithCallbackData("Get bank"),
        //            InlineKeyboardButton.WithCallbackData("Get currencies")
        //        }
        //    });


        //    ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
        //    {
        //        new KeyboardButton("Send location") {RequestLocation = true},
        //    })
        //    {
        //        ResizeKeyboard = true,
        //    };

        //    await BotClient.SendTextMessageAsync(Id, "Send your location", replyMarkup: replyKeyboardMarkup);
        //    await BotClient.SendTextMessageAsync(Id, "a", replyMarkup: inline);
        //}
    }
}