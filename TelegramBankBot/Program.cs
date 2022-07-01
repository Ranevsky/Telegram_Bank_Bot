using AutoMapper;

using Microsoft.Extensions.Configuration;

using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

using TelegramBankBot.Automapper;
using TelegramBankBot.DB;
using TelegramBankBot.DB.Interfaces;
using TelegramBankBot.Handlers;
using TelegramBankBot.Logger;

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
        try
        {
            ApplicationContext db = new();
            UOW = new UnitOfWork(db);
            await StartBot();
        }
        finally
        {
            UOW.Dispose();
        }
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

        Helper.Menu.MenuDescription<int, Action> menu = new();
        menu[1, "Send all"] = () =>
        {
            if (cts.IsCancellationRequested == true)
            {
                Console.WriteLine("End");
                return;
            }

            Console.WriteLine("Enter text or send 'exit'");
            string? text;
            do
            {
                text = Console.ReadLine();
            } while (string.IsNullOrEmpty(text));

            if (string.Equals(text, "exit", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            IEnumerable<Model.User>? users = UOW.Users.GetAll().AsEnumerable();

            try
            {
                Parallel.ForEach(users, async user =>
                {
                    await botClient.SendTextMessageAsync(user.Id, text);
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return;
            }

            Console.WriteLine("Success");
        };

        menu.Menu(0);

        // Send cancellation request to stop bot
        cts.Cancel();
    }

    public static IUnitOfWork UOW { get; private set; } = null!;
    public static IMapper Mapper { get; private set; } = AutoMapping.Mapper;
    private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Type == UpdateType.MyChatMember)
        {
            ChatMemberUpdated memberUpdate = update.MyChatMember!;
            ChatMember member = memberUpdate.NewChatMember;
            ChatMemberStatus status = member.Status;

            TelegramBankBot.Model.User user = Mapper.Map<TelegramBankBot.Model.User>(memberUpdate.From);

            switch (status)
            {
                case ChatMemberStatus.Kicked:
                {
                    // remove user in db
                    await UOW.Users.RemoveProtectedAsync(user);
                    Log.Info($"Removed '{user.Id} {user.Name}'");
                    break;
                }
                case ChatMemberStatus.Member:
                {
                    // add user in db
                    await UOW.Users.AddAsync(user);
                    Log.Info($"Added '{user.Id} {user.Name}'");
                    break;
                }
                default:
                {
                    Log.Error($"Chatmember status '{status}' not handling");
                    break;
                }
            }
            await UOW.SaveAsync();
            return;
        }

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