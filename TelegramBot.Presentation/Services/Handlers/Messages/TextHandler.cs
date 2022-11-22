using AutoMapper;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Presentation.Exceptions;
using TelegramBot.Presentation.Models.Args;

namespace TelegramBot.Presentation.Services.Handlers.Messages;

public class TextHandler : Handler<Message>
{
    private readonly Handler<TextArgs> _handler;
    private readonly ILogger<TextHandler> _logger;
    private readonly IMapper _mapper;

    public TextHandler(Handler<TextArgs> handler, IMapper mapper, ILogger<TextHandler> logger)
    {
        _handler = handler;
        _mapper = mapper;
        _logger = logger;
    }

    public override async Task HandleAsync(Message message)
    {
        var type = message.Type;
        if (type == MessageType.Text)
        {
            var textArgs = _mapper.Map<TextArgs>(message);

            _logger.LogInformation(textArgs.From, "send text '{Text}'", textArgs.Text);

            try
            {
                await _handler.HandleAsync(textArgs);
            }
            catch (HandlerNotFoundException)
            {
                _logger.LogError("Message type '{Type}' not found handler", type);
            }
        }
        else
        {
            await Successor.HandleAsync(message);
        }
    }
}