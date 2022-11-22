namespace Bank.Infrastructure.Extensions;

internal static class DateTimeExtensions
{
    public static long TickInSecond(this DateTime dateTime)
    {
        const int tickInSecond = 10_000_000;
        return dateTime.Ticks / tickInSecond;
    }
}