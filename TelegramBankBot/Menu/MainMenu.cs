
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBankBot;

public class MainMenu
{
    private Bot _bot;
    public MainMenu(Bot bot)
    {
        _bot = bot;
    }

    public static string Name { get; } = nameof(MainMenu);

    private static readonly ILogger log = Program.Log;

    private const string GEO = "geo";
    private const string BANKS = "banks";
    private const string CURR = "curr";
    private const string CURR_NEAR = "near";


    public static InlineKeyboardMarkup GetKeyboard()
    {
        var buttons = new InlineKeyboardButton[][]
        {
            new InlineKeyboardButton[]
            {
                InlineKeyboardButton.WithCallbackData("Geolocation", $"{Name}.{GEO}"),
            },
            new InlineKeyboardButton[]
            {
                InlineKeyboardButton.WithCallbackData("Banks", $"{Name}.{BANKS}"),
            },
            new InlineKeyboardButton[]
            {
                InlineKeyboardButton.WithCallbackData("Best currencies", $"{Name}.{CURR}"),
                InlineKeyboardButton.WithCallbackData("Best near currencies", $"{Name}.{CURR}.{CURR_NEAR}"),
            }
        };

        InlineKeyboardMarkup keyboard = new(buttons);
        return keyboard;
    }
    public void Handle(string[] args)
    {
        string arg1 = args[1];

        Func<Task> action = arg1 switch
        {
            GEO => GeolocationHandle,
            _ => UnknowHandle
        };

        action.Invoke();


        Task UnknowHandle()
        {
            log.Error($"I don't know '{args[1]}'");
            return Task.CompletedTask;
        }
        async Task GeolocationHandle()
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
            {
                new KeyboardButton("Send location") { RequestLocation = true },
            })
            {
                ResizeKeyboard = true,
            };

            Console.WriteLine("Reqiured geo");
            await _bot.SendMessageAsync("Geo", replyMarkup: replyKeyboardMarkup);
        }
    }
}