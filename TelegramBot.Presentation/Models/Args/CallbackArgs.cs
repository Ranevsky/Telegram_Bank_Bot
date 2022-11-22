using AutoMapper;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot.Presentation.Models.Args;

public class CallbackArgs : MessageArgs
{
    public string CallbackId { get; set; } = null!;
    public InlineKeyboardMarkup KeyboardMarkup { get; set; } = null!;

    public void Mapping(Profile profile)
    {
    }
}