using AutoMapper;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Application.Exceptions;
using TelegramBot.Presentation.Exceptions;
using User = TelegramBot.Domain.Entities.User;

namespace TelegramBot.Presentation.Services.Handlers.Updates;

public class MessageHandler : Handler<Update>
{
    private readonly Handler<Message> _handler;
    private readonly ILogger<MessageHandler> _logger;
    private readonly IMapper _mapper;

    public MessageHandler(Handler<Message> handler, IMapper mapper, ILogger<MessageHandler> logger)
    {
        _handler = handler;
        _mapper = mapper;
        _logger = logger;
    }

    public override async Task HandleAsync(Update update)
    {
        var type = update.Type;

        if (type == UpdateType.Message)
        {
            var message = update.Message!;

            try
            {
                await _handler.HandleAsync(message);
            }
            catch (HandlerNotFoundException)
            {
                _logger.LogError("Message type '{MessageType}' not implemented", message.Type);
            }
            catch (UserNotFoundException ex)
            {
                var user = _mapper.Map<User>(message.From);
                ex.SetData(user, message.Chat.Id);
                throw;
            }
        }
        else
        {
            await Successor.HandleAsync(update);
        }
    }
}