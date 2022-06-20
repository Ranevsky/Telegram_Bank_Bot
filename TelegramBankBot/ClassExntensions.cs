using System.Linq;

namespace TelegramBankBot;

public static class ClassExntensions
{
    public static string[] GetStrings(this string text)
    {
        return text.Split(' ')
            .Where(text => !string.IsNullOrWhiteSpace(text))
            .Select(text => text.Trim())
            .ToArray();
    }
}
