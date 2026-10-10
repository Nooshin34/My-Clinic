namespace Clinic.Application.Common;

public static class ClinicClock
{
    public static TimeZoneInfo Tehran { get; } = ResolveTehran();

    public static (DateTime StartUtc, DateTime EndUtc) TodayUtc(DateTime? utcNow = null)
    {
        var now = utcNow ?? DateTime.UtcNow;
        var localNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(now, DateTimeKind.Utc), Tehran);
        var startLocal = DateTime.SpecifyKind(localNow.Date, DateTimeKind.Unspecified);
        var endLocal = startLocal.AddDays(1);

        return (
            TimeZoneInfo.ConvertTimeToUtc(startLocal, Tehran),
            TimeZoneInfo.ConvertTimeToUtc(endLocal, Tehran));
    }

    public static DateTime AtLocalTime(DateTime localDate, int hour, int minute)
    {
        var local = DateTime.SpecifyKind(localDate.Date.AddHours(hour).AddMinutes(minute), DateTimeKind.Unspecified);
        return TimeZoneInfo.ConvertTimeToUtc(local, Tehran);
    }

    private static TimeZoneInfo ResolveTehran()
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById("Asia/Tehran");
        }
        catch (TimeZoneNotFoundException)
        {
            return TimeZoneInfo.FindSystemTimeZoneById("Iran Standard Time");
        }
    }
}
