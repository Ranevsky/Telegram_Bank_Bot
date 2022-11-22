using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Application.Interfaces;
using TelegramBot.Presentation.Models.Args;
using TelegramBot.Presentation.Services.Handlers;
using TelegramBot.Presentation.Services.Handlers.CallbackQueries;
using TelegramBot.Presentation.Services.Handlers.CallbackQueries.Bank;
using TelegramBot.Presentation.Services.Handlers.CallbackQueries.Settings;
using TelegramBot.Presentation.Services.Handlers.CallbackQueries.Settings.Currencies;
using TelegramBot.Presentation.Services.Handlers.ChatMember;
using TelegramBot.Presentation.Services.Handlers.Commands;
using TelegramBot.Presentation.Services.Handlers.Commands.Remove;
using TelegramBot.Presentation.Services.Handlers.Messages;
using TelegramBot.Presentation.Services.Handlers.Text;
using TelegramBot.Presentation.Services.Handlers.Updates;
using TextHandler = TelegramBot.Presentation.Services.Handlers.Messages.TextHandler;

namespace TelegramBot.Presentation.Extensions;

public static class TelegramHandlerServiceExtension
{
    // Handlers
    public static IServiceCollection AddHandlers(this IServiceCollection services)
    {
        services
            .AddCallbackHandlers()
            .AddCommandHandlers()
            .AddMessageHandlers()
            .AddTextHandlers()
            .AddUpdateHandlers()
            .AddChatMemberHandlers();

        return services;
    }

    private static void AddHandler<T>(IServiceCollection services) where T : class
    {
        services.AddSingleton<T>();
    }

    private static void AddHandler<T>(IServiceCollection services, Func<IServiceProvider, T> factory) where T : class
    {
        services.AddSingleton(factory);
    }

    public static IServiceCollection AddCallbackHandlers(this IServiceCollection services)
    {
        AddHandler<GeolocationCallback>(services);
        AddHandler<CurrencySetCallback>(services);
        AddHandler<CurrencyOperationCallback>(services);
        AddHandler<DepartmentCallback>(services);

        // CurrencyCallback
        AddHandler(services, serviceProvider =>
        {
            Handler<CallbackArgs> currSetCallback = serviceProvider.GetRequiredService<CurrencySetCallback>();
            Handler<CallbackArgs> currOperationCallback =
                serviceProvider.GetRequiredService<CurrencyOperationCallback>();

            currSetCallback.Successor = currOperationCallback;

            var bot = serviceProvider.GetRequiredService<ITelegramBotClient>();
            var uow = serviceProvider.GetRequiredService<IUnitOfWork>();

            CurrencyCallback currCallBack = new(currSetCallback, bot, uow);

            return currCallBack;
        });

        // SettingsCallback
        AddHandler(services, serviceProvider =>
        {
            Handler<CallbackArgs> geoCallback = serviceProvider.GetRequiredService<GeolocationCallback>();
            Handler<CallbackArgs> currCallback = serviceProvider.GetRequiredService<CurrencyCallback>();

            geoCallback.Successor = currCallback;

            var bot = serviceProvider.GetRequiredService<ITelegramBotClient>();

            var uow = serviceProvider.GetRequiredService<IUnitOfWork>();

            SettingsCallback settingsCallback = new(geoCallback, bot, uow);

            return settingsCallback;
        });

        AddHandler<NearBankCallback>(services);
        AddHandler<BestCurrencyExchange>(services);

        // BankCallback
        AddHandler(services, serviceProvider =>
        {
            Handler<CallbackArgs> nearBankCallback = serviceProvider.GetRequiredService<NearBankCallback>();
            Handler<CallbackArgs> bestCurrencyExchangeBankCallback =
                serviceProvider.GetRequiredService<BestCurrencyExchange>();

            nearBankCallback.Successor = bestCurrencyExchangeBankCallback;

            BankCallback bankCallback = new(nearBankCallback);

            return bankCallback;
        });


        // MainCallback
        AddHandler(services, serviceProvider =>
        {
            Handler<CallbackArgs> settingsCallback = serviceProvider.GetRequiredService<SettingsCallback>();

            var bot = serviceProvider.GetRequiredService<ITelegramBotClient>();
            var uow = serviceProvider.GetRequiredService<IUnitOfWork>();

            MainCallback mainCallBack = new(settingsCallback, bot, uow);

            return mainCallBack;
        });

        // Handler<CallbackArgs>
        AddHandler(services, serviceProvider =>
        {
            Handler<CallbackArgs> mainCallback = serviceProvider.GetRequiredService<MainCallback>();
            Handler<CallbackArgs> settingsCallback = serviceProvider.GetRequiredService<SettingsCallback>();
            Handler<CallbackArgs> bankCallback = serviceProvider.GetRequiredService<BankCallback>();
            Handler<CallbackArgs> departmentCallback = serviceProvider.GetRequiredService<DepartmentCallback>();

            mainCallback.Successor = settingsCallback;
            settingsCallback.Successor = bankCallback;
            bankCallback.Successor = departmentCallback;

            return mainCallback;
        });

        return services;
    }

    public static IServiceCollection AddCommandHandlers(this IServiceCollection services)
    {
        AddHandler<StartCommand>(services);
        AddHandler<RemoveKeyboard>(services);

        // RemoveCommand
        AddHandler(services, serviceProvider =>
        {
            Handler<CommandArgs> removeKeyboard = serviceProvider.GetRequiredService<RemoveKeyboard>();

            RemoveCommand removeCommand = new(removeKeyboard);

            return removeCommand;
        });
        AddHandler<TestCommand>(services);

        // Handler<CommandArgs>
        AddHandler(services, serviceProvider =>
        {
            Handler<CommandArgs> registerCommand = serviceProvider.GetRequiredService<StartCommand>();
            Handler<CommandArgs> removeKeyboardCommand = serviceProvider.GetRequiredService<RemoveCommand>();
            Handler<CommandArgs> testCommand = serviceProvider.GetRequiredService<TestCommand>();

            registerCommand.Successor = removeKeyboardCommand;
            removeKeyboardCommand.Successor = testCommand;

            return registerCommand;
        });

        return services;
    }

    public static IServiceCollection AddMessageHandlers(this IServiceCollection services)
    {
        AddHandler<LocationHandler>(services);
        AddHandler<TextHandler>(services);

        // Handler<Message>
        AddHandler(services, serviceProvider =>
        {
            Handler<Message> textHandler = serviceProvider.GetRequiredService<TextHandler>();
            Handler<Message> locationHandler = serviceProvider.GetRequiredService<LocationHandler>();

            textHandler.Successor = locationHandler;

            return textHandler;
        });

        return services;
    }

    public static IServiceCollection AddTextHandlers(this IServiceCollection services)
    {
        AddHandler<CommandHandler>(services);
        AddHandler<Services.Handlers.Text.TextHandler>(services);

        // Handler<TextArgs>
        AddHandler(services, serviceProvider =>
        {
            Handler<TextArgs> commandHandler = serviceProvider.GetRequiredService<CommandHandler>();
            Handler<TextArgs> textHandler = serviceProvider.GetRequiredService<Services.Handlers.Text.TextHandler>();

            commandHandler.Successor = textHandler;

            return commandHandler;
        });

        return services;
    }

    public static IServiceCollection AddUpdateHandlers(this IServiceCollection services)
    {
        AddHandler<MessageHandler>(services);
        AddHandler<CallbackQueryHandler>(services);
        AddHandler<MyChatMemberHandler>(services);

        // Handler<Update>
        AddHandler(services, serviceProvider =>
        {
            Handler<Update> messageHandler = serviceProvider.GetRequiredService<MessageHandler>();
            Handler<Update> callbackHandler = serviceProvider.GetRequiredService<CallbackQueryHandler>();
            Handler<Update> chatMemberHandler = serviceProvider.GetRequiredService<MyChatMemberHandler>();

            messageHandler.Successor = callbackHandler;
            callbackHandler.Successor = chatMemberHandler;

            return messageHandler;
        });

        return services;
    }

    public static IServiceCollection AddChatMemberHandlers(this IServiceCollection services)
    {
        AddHandler<KickedMember>(services);
        AddHandler<Member>(services);

        // Handler<ChatMemberUpdated>
        AddHandler(services, serviceProvider =>
        {
            Handler<ChatMemberUpdated> memberHandler = serviceProvider.GetRequiredService<Member>();
            Handler<ChatMemberUpdated> kickedMemberHandler = serviceProvider.GetRequiredService<KickedMember>();

            memberHandler.Successor = kickedMemberHandler;

            return memberHandler;
        });

        return services;
    }
}