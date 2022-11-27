using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Application.Interfaces;
using TelegramBot.Presentation.Models.Args;
using TelegramBot.Presentation.Services.Handlers.CallbackQueries;

namespace TelegramBot.Presentation.Services.Handlers.Commands;

public class StartCommand : Handler<CommandArgs>
{
    private const string CommandName = "START";
    private readonly ITelegramBotClient _bot;
    private readonly ILogger<StartCommand> _logger;

    private readonly IUnitOfWork _uow;

    public StartCommand(IUnitOfWork uow, ILogger<StartCommand> logger, ITelegramBotClient bot)
    {
        _uow = uow;
        _logger = logger;
        _bot = bot;
    }

    public override async Task HandleAsync(CommandArgs args)
    {
        if (args.GetArg() == CommandName)
        {
            args.ArgsIteration++;

            var isAdded = await _uow.Users.AddWithCheckExistsAsync(args.From);
            if (isAdded)
            {
                _logger.LogInformation(args.From, "registered");
            }

            InlineKeyboardMarkup keyboard = new(InlineKeyboardButton.WithCallbackData("Start", MainCallback.Name));

            await _bot.SendTextMessageAsync(args.ChatId, "Let's go", replyMarkup: keyboard);
        }
        else
        {
            await Successor.HandleAsync(args);
        }
    }
}