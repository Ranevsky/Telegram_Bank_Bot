using AutoMapper;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Application.Interfaces;
using User = TelegramBot.Domain.Entities.User;

namespace TelegramBot.Presentation.Services.Handlers.ChatMember;

public class Member : Handler<ChatMemberUpdated>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _uow;

    public Member(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public override async Task HandleAsync(ChatMemberUpdated memberUpdate)
    {
        var status = memberUpdate.NewChatMember.Status;

        if (status == ChatMemberStatus.Member)
        {
            var userTg = memberUpdate.From;
            var user = _mapper.Map<User>(userTg);

            await _uow.Users.AddWithCheckExistsAsync(user);
        }
        else
        {
            await Successor.HandleAsync(memberUpdate);
        }
    }
}