using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Extensions;
using Telegram.Bot.Polling;
using Telegram.Bot.Requests;

namespace TelegramBankBot;

public class MainMenu
{
    private readonly Bot _bot;
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
        InlineKeyboardButton[][]? buttons = new InlineKeyboardButton[][]
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
    public async Task HandleAsync(string[] args, int messageId)
    {
        string arg1 = args[1];

        Func<Task> action = arg1 switch
        {
            GEO => GeolocationHandle,
            BANKS => BanksHandle,
            CURR => CurrMennu,
            _ => UnknowHandle
        };

        await action.Invoke();



        Task CurrMennu()
        {
            if (args.Length > 2)
            {
                NearCurr();
            }
            else
            {
                BestCurr();
            }
            return Task.CompletedTask;



            void BestCurr()
            {
#warning Implement BestCurr
            }
            void NearCurr()
            {
#warning Implement NearCurr
            }
        }
        async Task BanksHandle()
        {
#warning Implement BanksHandle
            await Bot.BotClient.EditMessageTextAsync(_bot.Id, messageId, "ABC");
        }
        Task UnknowHandle()
        {
            throw new Exception($"'{args[1]}' not implemented");
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
            await _bot.SendMessageAsync("Send your Geo\nOr send '/remove_keyboard' to remove keyboard", replyMarkup: replyKeyboardMarkup);
        }
    }
}