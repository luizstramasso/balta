namespace Fina.Core.Common;

public static class DateTimeExtention
{
    public static DateTime GetFirstDayOfMonth(this DateTime date, int? year = null, int? month = null)
        => new(year ?? date.Year, month ?? date.Month, 1);

    public static DateTime GetLastDayOfMonth(this DateTime date, int? year = null, int? month = null)
        => new DateTime(year ?? date.Year, month ?? date.Month, 1)
            .AddMonths(1)
            .AddDays(-1);
}