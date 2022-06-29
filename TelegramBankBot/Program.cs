using Microsoft.Extensions.Configuration;

using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramBankBot;
//if (message.Text is not { } messageText)

public class Program
{
    public static IConfiguration Configuration { get; private set; } = null!;
    public static ILogger Log { get; private set; } = null!;
    public static async Task Main()
    {
        Log = new ConsoleLogger();

        GetConfiguration();
        await StartBot();

        Helper.Menu.MenuDescription<int, Action> menu = new();
        //menu[1, "Add"] = () => AddUser();
        menu.Menu(0);
    }
    private static void GetConfiguration()
    {
        ConfigurationBuilder? builder = new();
        builder.SetBasePath(Directory.GetCurrentDirectory());
        builder.AddJsonFile("appsettings.json");
        Configuration = builder.Build();
    }
    private static async Task StartBot()
    {
        ITelegramBotClient botClient = Bot.BotClient;

        using CancellationTokenSource? cts = new();

        // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
        ReceiverOptions? receiverOptions = new()
        {
            AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
        };

        botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            pollingErrorHandler: HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: cts.Token
        );

        Telegram.Bot.Types.User? me = await botClient.GetMeAsync();

        Log.Info($"Start listening for @{me.Username}");
        Console.ReadLine();

        // Send cancellation request to stop bot
        cts.Cancel();
    }
    private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        Bot? bot = update.Type switch
        {
            UpdateType.Message => new BotMessageHandler(update),
            UpdateType.CallbackQuery => new BotCallbackHandler(update),
            _ => null
        };


        if (bot == null)
        {
            Log.Warning($"'{update.Type}' is not implemented");
            return;
        }

        try
        {
            await bot.HandleAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
        }
    }
    private static Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        string? ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Log.Error(ErrorMessage);
        return Task.CompletedTask;
    }
}