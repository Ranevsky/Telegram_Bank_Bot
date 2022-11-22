using TelegramBot.Domain.Entities;

namespace Microsoft.Extensions.Logging;

public static class LoggerExtension
{
    private static void Log(
        ILogger logger,
        LogLevel logLevel,
        string firstName,
        long id,
        string? message,
        params object[] args)
    {
        if (message is null)
        {
            return;
        }

        // args is not null (args.Length = 0 or any)
        var capacity = args.Length;
        List<object> list = new(2 + capacity)
        {
            firstName, id
        };

        if (capacity != 0)
        {
            list.AddRange(args);
        }

        var msg = "User '{FirstName}' with id '{Id}', " + message;
        logger.Log(logLevel, msg, list.ToArray());
    }

    public static void LogInformation(this ILogger logger, User user, string? message, params object[] args)
    {
        Log(logger, LogLevel.Information, user.FirstName, user.Id, message, args);
    }

    public static void LogInformation(
        this ILogger logger,
        Telegram.Bot.Types.User user,
        string? message,
        params object[] args)
    {
        Log(logger, LogLevel.Information, user.FirstName, user.Id, message);
    }

    public static void LogWarning(
        this ILogger logger,
        Telegram.Bot.Types.User user,
        string? message,
        params object[] args)
    {
        Log(logger, LogLevel.Warning, user.FirstName, user.Id, message);
    }

    public static void LogWarning(this ILogger logger, User user, string? message, params object[] args)
    {
        Log(logger, LogLevel.Warning, user.FirstName, user.Id, message);
    }
}