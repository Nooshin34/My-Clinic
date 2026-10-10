using Clinic.Domain.Enums;

namespace Clinic.Application.Common;

public static class ClinicEnums
{
    public static Gender? ParseGender(string? value) => Normalize(value) switch
    {
        "female" or "زن" => Gender.Female,
        "male" or "مرد" => Gender.Male,
        "unknown" or "نامشخص" => Gender.Unknown,
        _ => null
    };

    public static AppointmentStatus? ParseStatus(string? value) => Normalize(value) switch
    {
        "scheduled" or "برنامهریزی شده" or "برنامه‌ریزی‌شده" => AppointmentStatus.Scheduled,
        "completed" or "انجام شده" or "انجام‌شده" => AppointmentStatus.Completed,
        "cancelled" or "canceled" or "لغو شده" or "لغوشده" => AppointmentStatus.Cancelled,
        _ => null
    };

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToLowerInvariant();
}
