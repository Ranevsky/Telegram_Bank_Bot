using AutoMapper;
using Telegram.Bot;
using TelegramBot.Presentation.Exceptions;
using TelegramBot.Presentation.Models.Args;

namespace TelegramBot.Presentation.Services.Handlers.Text;

public class CommandHandler : Handler<TextArgs>
{
    private readonly ITelegramBotClient _bot;
    private readonly Handler<CommandArgs> _handler;
    private readonly IMapper _mapper;

    public CommandHandler(Handler<CommandArgs> handler, ITelegramBotClient bot, IMapper mapper)
    {
        _handler = handler;
        _bot = bot;
        _mapper = mapper;
    }

    public override async Task HandleAsync(TextArgs textArgs)
    {
        if (textArgs.Text.StartsWith('/'))
        {
            var commandArgs = _mapper.Map<CommandArgs>(textArgs);

            try
            {
                await _handler.HandleAsync(commandArgs);
            }
            catch (HandlerNotFoundException)
            {
                var command = string.Join('_', commandArgs);

                await _bot.SendTextMessageAsync(
                    textArgs.ChatId,
                    $"Command '/{command}' not found",
                    replyToMessageId: textArgs.MessageId);
            }
            catch (CommandArgumentNotFoundException ex)
            {
                await _bot.SendTextMessageAsync(
                    textArgs.ChatId,
                    ex.Message,
                    replyToMessageId: textArgs.MessageId);
            }
        }
        else
        {
            await Successor.HandleAsync(textArgs);
        }
    }
}