using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

using TelegramBankBot.Handlers;


namespace TelegramBankBot.Handlers.Menu;

public class MainMenu : MenuHandler
{
    private const string GEO = "geo";
    private const string BANKS = "banks";
    private const string CURR = "curr";
    private const string CURR_NEAR = "near";
    private const string NAME = nameof(MainMenu);
    public static InlineKeyboardMarkup GetKeyboard()
    {
        InlineKeyboardButton[][]? buttons = new InlineKeyboardButton[][]
        {
            new InlineKeyboardButton[]
            {
                InlineKeyboardButton.WithCallbackData("Geolocation", $"{NAME}.{GEO}"),
            },
            new InlineKeyboardButton[]
            {
                InlineKeyboardButton.WithCallbackData("Banks", $"{NAME}.{BANKS}"),
            },
            new InlineKeyboardButton[]
            {
                InlineKeyboardButton.WithCallbackData("Best currencies", $"{NAME}.{CURR}"),
                InlineKeyboardButton.WithCallbackData("Best near currencies", $"{NAME}.{CURR}.{CURR_NEAR}"),
            }
        };

        InlineKeyboardMarkup keyboard = new(buttons);
        return keyboard;
    }
    
    public MainMenu(Bot bot, string[] args) : base(bot)
    {
        _args = args;
    }
    private readonly string[] _args;

    public override async Task HandleAsync()
    {
        string arg1 = _args[1];

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
            if (_args.Length > 2)
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
            await Bot.BotClient.EditMessageTextAsync(Bot.Id, Bot.MessageId, "ABC");
        }
        Task UnknowHandle()
        {
            throw new Exception($"'{_args[1]}' not implemented");
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
            await Bot.SendMessageAsync("Send your Geo\nOr send '/remove_keyboard' to remove keyboard", replyMarkup: replyKeyboardMarkup);
        }
    }
}