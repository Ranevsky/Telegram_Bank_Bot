using AutoMapper;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Application.Exceptions;
using TelegramBot.Presentation.Exceptions;
using User = TelegramBot.Domain.Entities.User;

namespace TelegramBot.Presentation.Services.Handlers.Updates;

public class MyChatMemberHandler : Handler<Update>
{
    private readonly Handler<ChatMemberUpdated> _handler;
    private readonly ILogger<MyChatMemberHandler> _logger;
    private readonly IMapper _mapper;

    public MyChatMemberHandler(Handler<ChatMemberUpdated> handler, ILogger<MyChatMemberHandler> logger, IMapper mapper)
    {
        _handler = handler;
        _logger = logger;
        _mapper = mapper;
    }

    public override async Task HandleAsync(Update update)
    {
        var type = update.Type;
        if (type == UpdateType.MyChatMember)
        {
            var memberUpdate = update.MyChatMember!;

            try
            {
                await _handler.HandleAsync(memberUpdate);
            }
            catch (HandlerNotFoundException)
            {
                _logger.LogError("Member type '{Status}' not found", memberUpdate.NewChatMember.Status);
            }
            catch (UserNotFoundException ex)
            {
                var user = _mapper.Map<User>(memberUpdate.From);
                ex.SetData(user, memberUpdate.Chat.Id);
                throw;
            }
        }
        else
        {
            await Successor.HandleAsync(update);
        }
    }
}