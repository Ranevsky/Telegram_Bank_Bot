using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBankBot;

public class CommandHandler 
{
    private Bot _bot;
    private ILogger _log = Program.Log;
    public CommandHandler(Bot bot)
    {
        _bot = bot;
    }

    public async Task HandleAsync(string commandString)
    {
        string[] commandArgs = commandString.GetStrings();
        string command = commandArgs[0];

        switch (command.ToUpper())
        {
            case "/TEST":
                await TestCommand();
                break;

            default:
                _log.Warning($"Command '{commandString}' not implemented");
                return;
        }

        async Task TestCommand()
        {
            InlineKeyboardMarkup keyBoard = MainMenu.GetKeyboard();

            await _bot.SendMessageAsync(
                text: "TEST",
                replyMarkup: keyBoard);
        }
    }
}
