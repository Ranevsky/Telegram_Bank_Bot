
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

public class BotHandleCallback : Bot
{
    private readonly CallbackQuery _callback = null!;
    #region ctor's
    public BotHandleCallback(CallbackQuery callbackQuery)
        : base(callbackQuery.Message!.Chat.Id)
    {
        _callback = callbackQuery;
    }

    public BotHandleCallback(Update update)
        : this(update.CallbackQuery ?? throw new NullReferenceException(nameof(update.CallbackQuery)))
    {

    }
    #endregion

    public async Task HandleCallbackAsync()
    {
        Message msg = _callback.Message!;
        // Data -> Id кнопки?
        // Text -> Текст сообщения
        // MessageId -> Id сообщения
        string text = $"{msg.Text} {msg.MessageId} {_callback.Data}";
        Log.Info(text);

        switch (_callback.Data!.ToUpper())
        {
            case "TIME":
            {
                await SendMessageAsync($"Time: {DateTime.Now.ToString("HH:mm:ss")}");
                break;
            }

            case "LOCATION":
            {
                await SetLocation();
                break;
            }

            default:
            {
                Log.Error($"CallBack '{_callback.Data}' not found");
                break;
            }
        }
    
        async Task SetLocation()
        {
            InlineKeyboardMarkup inline = new(new[]
            {
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("Set Location")
                },
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("Get bank"),
                    InlineKeyboardButton.WithCallbackData("Get currencies")
                }
            });


            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
            {
                new KeyboardButton("Send location") {RequestLocation = true},
            })
            {
                ResizeKeyboard = true,
            };

            await BotClient.SendTextMessageAsync(Id, "Send your location", replyMarkup: replyKeyboardMarkup);
            await BotClient.SendTextMessageAsync(Id, "", replyMarkup: inline);
        }
    }

    

    public static async void SendCallBack(long id)
    {
        //await BotClient.AnswerCallbackQueryAsync("1");


        var inlineKeyboard = new InlineKeyboardMarkup(new[]
        {
            InlineKeyboardButton.WithCallbackData("Set Location", "LOCATION"),
            InlineKeyboardButton.WithCallbackData("Time", "TIME"),
        });

        await BotClient.SendTextMessageAsync(
            chatId: id,
            text: "asd",
            replyMarkup: inlineKeyboard);
    }
}