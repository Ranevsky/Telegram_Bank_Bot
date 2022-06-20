using Microsoft.Extensions.Configuration;

using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramBankBot;
//if (message.Text is not { } messageText)
//var chatId = message.Chat.Id;

//Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");
public class Program
{
    public static IConfiguration Configuration { get; private set; } = null!;
    private static void GetConfiguration()
    {
        ConfigurationBuilder? builder = new();
        builder.SetBasePath(Directory.GetCurrentDirectory());
        builder.AddJsonFile("appsettings.json");
        Configuration = builder.Build();
    }
    private static ILogger _log = null!;
    public static async Task Main()
    {
        _log = new ConsoleLogger();

        GetConfiguration();
        await StartBot();

        Helper.Menu.MenuDescription<int, Action> menu = new();
        //menu[1, "Add"] = () => AddUser();
        menu.Menu(0);
    }

    private static async Task StartBot()
    {
        string token = Configuration.GetSection("Telegram")["Token"];
        TelegramBotClient? botClient = new(token);

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

        _log.Info($"Start listening for @{me.Username}");
        Console.ReadLine();

        // Send cancellation request to stop bot
        cts.Cancel();
    }

    // Обработчик сообщений
    private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        switch (update.Type)
        {
            case UpdateType.Message:
            {
                //update.
                BotHandleMessage bot = new(botClient, update);
                await bot.HandleMessageAsync();
                break;
            }

            case UpdateType.CallbackQuery:
            {
                _log.Info($"Callback");
                //update.CallbackQuery.From.Id
                //return;
                //update.CallbackQuery.Message.Chat.Id
                BotHandleCallback bot = new(botClient, update);
                
                //await botClient.SendTextMessageAsync(update.CallbackQuery.From.Id, "asd");
                //await HandleCallBackQuery(botClient, update.CallbackQuery!);
                break;
            }

            default:
            {
                _log.Warning($"'{update.Type}' is not implemented");
                return;
            }
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

        _log.Error(ErrorMessage);
        return Task.CompletedTask;
    }

    private static async Task HandleCallBackQuery(ITelegramBotClient botClient, CallbackQuery message)
    {
        string text = $"{message.ChatInstance} {message.Data}";
        await botClient.SendTextMessageAsync(
            chatId: message.Message!.Chat.Id,
            text: text);
    }
}