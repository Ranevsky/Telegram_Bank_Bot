using Telegram.Bot.Types;

using TelegramBankBot.Handlers.Menu;

namespace TelegramBankBot.Handlers;

public class BotCallbackHandler : Bot
{
    private readonly CallbackQuery _callback = null!;
    #region ctor's
    public BotCallbackHandler(CallbackQuery callbackQuery)
        : base(callbackQuery.Message ?? throw new NullReferenceException(nameof(callbackQuery)))
    {
        _callback = callbackQuery;
    }


    private void InitializeDictionary(string[] args)
    {
        dict = new()
        {
#warning return instance and use abstraction (instance).HandleAsync();
            { nameof(MainMenu), new MainMenu(this, args) }
        };
    }

    public BotCallbackHandler(Update update)
        : this(update.CallbackQuery ?? throw new NullReferenceException(nameof(update.CallbackQuery)))
    {

    }
    #endregion

    private Dictionary<string, MenuHandler> dict = null!;
    public override async Task HandleAsync()
    {
        Message msg = _callback.Message!;

        string text = $"{msg.Text} {msg.MessageId} {_callback.Data}";
        Log.Info(text);

        string[] args = _callback.Data!.Split('.');
        InitializeDictionary(args);
        try
        {
            if (dict.TryGetValue(args[0], out var instace))
            {
                await instace.HandleAsync();
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