using Microsoft.Extensions.Configuration;

using Telegram.Bot.Types;
using Telegram.Bot;

using TelegramBankBot.Model;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;

namespace TelegramBankBot;

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
    public static async Task Main()
    {
        GetConfiguration();
        await StartBot();

        Helper.Menu.MenuDescription<int, Action> menu = new();
        //menu[1, "Add"] = () => AddUser();
        menu.Menu(0);
    }

    static async Task StartBot()
    {
        string token = Configuration.GetSection("Telegram")["Token"];
        var botClient = new TelegramBotClient(token);

        using var cts = new CancellationTokenSource();

        // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
        };

        botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            pollingErrorHandler: HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: cts.Token
        );

        var me = await botClient.GetMeAsync();

        Console.WriteLine($"Start listening for @{me.Username}");
        Console.ReadLine();

        // Send cancellation request to stop bot
        cts.Cancel();
    }

    // Обработчик сообщений
    static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        // Only process Message updates: https://core.telegram.org/bots/api#message
        //if (update.Message is not { } message)
        //    return;

        //if(update.Type == UpdateType.Message)

        Chat chat = update.Message!.Chat;
        Console.WriteLine($"{chat.Id} {chat.FirstName}");

        switch (update.Type)
        {
            case UpdateType.Message:
            {
                HandleMessage(botClient, update.Message!);
                break;
            }
            
            default:
            {
                Console.WriteLine($"ERROR: '{update.Type}' is not implemented");
                return;
            }
        }



        // Only process text messages
        //if (message.Text is not { } messageText)
        //    return;

        //var chatId = message.Chat.Id;

        //Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

        // Echo received message text
        //Message sentMessage = await botClient.SendTextMessageAsync(
        //    chatId: chatId,
        //    text: "You said:\n" + messageText,
        //    cancellationToken: cancellationToken);
    }

    // Обработчик ошибок
    static Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }


    static async void HandleMessage(ITelegramBotClient botClient, Message message)
    {
        switch (message.Type)
        {
            case MessageType.Text:
            {
                HandleMessageText(botClient, message);
                break;
            }

            case MessageType.Dice:
            {
                
                await botClient.SendTextMessageAsync(
                    message.Chat.Id,
                    message.Dice!.Value.ToString()
                    );
                break;
            }
        


            default:
            {
                Console.WriteLine($"ERROR: '{message.Type}' is not implemented");
                return;
            }
        }
        
    }
    static void HandleMessageText(ITelegramBotClient botClient, Message message)
    {
        Chat chat = message.Chat;
        string msg = message.Text!; // +?
        string? firstName = chat.FirstName; // +
        string? lastName = chat.LastName; // -
        string? username = chat.Username; // -
        long id = message.Chat.Id; // +
        Console.WriteLine($"|{firstName}| |{lastName}| |{username}| |{id}| |{msg}|");
    }

}
