namespace BankAPI.Extensions;

internal static class DateTimeExtensions
{
    public static long TickInSecond(this DateTime dateTime)
    {
        const int tickInSecond = 10000000;
        return dateTime.Ticks / tickInSecond;
    }
}
