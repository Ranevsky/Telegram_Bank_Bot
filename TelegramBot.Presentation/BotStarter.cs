using AutoMapper;
using Bank.Application.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Application.Exceptions;
using TelegramBot.Application.Interfaces;
using TelegramBot.Presentation.Exceptions;
using TelegramBot.Presentation.Services.Handlers;
using User = TelegramBot.Domain.Entities.User;

namespace TelegramBot.Presentation;

public class BotStarter
{
    private readonly ITelegramBotClient _bot;
    private readonly ILogger<BotStarter> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public BotStarter(
        ITelegramBotClient botClient,
        ILogger<BotStarter> logger,
        IServiceScopeFactory scopeFactory
    )
    {
        _bot = botClient;
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    public async Task StartBotAsync()
    {
        using CancellationTokenSource cts = new();

        // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
        ReceiverOptions receiverOptions = new()
        {
            AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
        };

        await _bot.ReceiveAsync(
            HandleUpdateAsync,
            HandlePollingErrorAsync,
            receiverOptions,
            cts.Token);

        var me = await _bot.GetMeAsync(cts.Token);

        _logger.LogInformation("Start listening for @{MeUsername}", me.Username);

        // Send cancellation request to stop bot
        cts.Cancel();
    }

    private Task HandlePollingErrorAsync(
        ITelegramBotClient botClient,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        _logger.LogError("{Msg}", errorMessage);
        return Task.CompletedTask;
    }

    private async Task HandleUpdateAsync(
        ITelegramBotClient botClient,
        Update update,
        CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<Handler<Update>>();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

        try
        {
            await handler.HandleAsync(update);
        }
        catch (HandlerNotFoundException)
        {
            _logger.LogError("Update type '{UpdateType}' is not implemented", update.Type);
        }
        catch (UserNotFoundException ex) when (ex.User is not null)
        {
            _logger.LogError(
                "User '{UserFirstName}' with id = '{UserId}' not found",
                ex.User.FirstName,
                ex.User.Id);

            var user = mapper.Map<User>(ex.User);

            await uow.Users.AddAsync(user);
            await uow.SaveAsync();

            await HandleUpdateAsync(botClient, update, cancellationToken);
        }
        catch (NearCityNotFoundException ex)
        {
            _logger.LogError("{Msg}", ex.Message);
        }
        catch (CityNotSupportedException ex)
        {
            _logger.LogError("{Msg}", ex.Message);
        }
        catch (ApiRequestException ex)
        {
            // The check is dumb, but I do not know how to check if the message has changed
            const string msgMessageNotModified =
                "Bad Request: message is not modified: specified new message content and reply markup are exactly the same as a current content and reply markup of the message";
            const string msgBotWasBlocked = "Forbidden: bot was blocked by the user";
            if (ex.Message == msgMessageNotModified)
            {
                _logger.LogWarning("Message edit not have new data");
            }
            else if (ex.Message == msgBotWasBlocked)
            {
                _logger.LogWarning("The untraceable user is working with the functionality");
            }
        }
    }
}