using Telegram.Bot.Types;

namespace TelegramBankBot.Exntensions;

public static class ClassExntensions
{
    public static string[] GetStrings(this string text)
    {
        return text.Split(' ')
            .Where(text => !string.IsNullOrWhiteSpace(text))
            .Select(text => text.Trim())
            .ToArray();
    }
    public static double Diff(this Location loc, Location location)
    {
        double p = Math.PI / 180;
        double diff = 0.5 - (Math.Cos((location.Latitude - loc.Latitude) * p) / 2) + (Math.Cos(loc.Latitude * p) * Math.Cos(location.Latitude * p) * (1 - Math.Cos((location.Longitude - loc.Longitude) * p)) / 2);
        return 12742 * Math.Asin(Math.Sqrt(diff));
    }
}
