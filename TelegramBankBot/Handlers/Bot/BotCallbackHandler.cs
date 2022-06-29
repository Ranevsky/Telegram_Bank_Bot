
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram;
using Telegram.Bot.Args;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Extensions;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Requests;
using Telegram.Bot.Types.InlineQueryResults;

namespace TelegramBankBot;

public class BotCallbackHandler : Bot
{
    private readonly CallbackQuery _callback = null!;
    #region ctor's
    public BotCallbackHandler(CallbackQuery callbackQuery)
        : base(callbackQuery.Message!.Chat.Id)
    {
        _callback = callbackQuery;

        InitializeDictionary(this);
    }

    private void InitializeDictionary(Bot bot)
    {
        dict = new()
        {
            { nameof(MainMenu), args => { new MainMenu(this).Handle(args); } }
        };
    }

    public BotCallbackHandler(Update update)
        : this(update.CallbackQuery ?? throw new NullReferenceException(nameof(update.CallbackQuery)))
    {

    }
    #endregion

    Dictionary<string, Action<string[]>> dict = null!;
    public override async Task HandleAsync()
    {
        Message msg = _callback.Message!;
        // Data -> Id кнопки?
        // Text -> Текст сообщения
        // MessageId -> Id сообщения
        string text = $"{msg.Text} {msg.MessageId} {_callback.Data}";
        Log.Info(text);


        string[] args = _callback.Data!.Split('.');

        if (dict.TryGetValue(args[0], out var act))
        {
            act.Invoke(args);
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