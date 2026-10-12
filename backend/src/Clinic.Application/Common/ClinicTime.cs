namespace Clinic.Application.Common;

public static class ClinicTime
{
    public static readonly TimeSpan IranOffset = TimeSpan.FromMinutes(210);

    public static DateTime ToUtc(DateOnly date, TimeOnly time)
    {
        var local = date.ToDateTime(time, DateTimeKind.Unspecified);
        return DateTime.SpecifyKind(local - IranOffset, DateTimeKind.Utc);
    }

    public static (DateOnly Date, TimeOnly Time) FromUtc(DateTime utc)
    {
        var local = DateTime.SpecifyKind(utc, DateTimeKind.Utc).Add(IranOffset);
        return (DateOnly.FromDateTime(local), new TimeOnly(local.Hour, local.Minute));
    }

    public static DateTime Minute(DateTime utc)
    {
        var value = utc.Kind == DateTimeKind.Local ? utc.ToUniversalTime() : DateTime.SpecifyKind(utc, DateTimeKind.Utc);
        return new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, 0, DateTimeKind.Utc);
    }

    public static DateOnly Today() => DateOnly.FromDateTime(DateTime.UtcNow.Add(IranOffset));
}
