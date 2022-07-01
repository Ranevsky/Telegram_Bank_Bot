using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

using TelegramBankBot.Exntensions;
using TelegramBankBot.Menu;

namespace TelegramBankBot.Handlers;

public class CommandHandler : Handler
{
    private readonly string _commandString;
    public CommandHandler(Bot bot, string commandString) : base(bot)
    {
        _commandString = commandString;
    }

    public override async Task HandleAsync()
    {
        string[] commandArgs = _commandString.GetStrings();
        string command = commandArgs[0];

        switch (command.ToUpper())
        {
            case "/TEST":
                await TestCommand();
                break;
#warning implement /start

            case "/REMOVE_KEYBOARD":
                await RemoveKeyboard();
                break;

            default:
                Log.Warning($"Command '{_commandString}' not implemented");
                return;
        }
        async Task RemoveKeyboard()
        {
            await Bot.SendMessageAsync(
                text: "Keyboard removed",
                replyMarkup: new ReplyKeyboardRemove());
        }
        async Task TestCommand()
        {
            InlineKeyboardMarkup keyBoard = MainMenu.GetKeyboard();

            Message? msg = await Bot.SendMessageAsync(
                text: "TEST",
                replyMarkup: keyBoard);
        }
    }
}
