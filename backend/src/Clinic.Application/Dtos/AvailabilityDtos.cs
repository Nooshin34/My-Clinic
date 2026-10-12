namespace Clinic.Application.Dtos;

public sealed record AvailabilitySlotDto(
    Guid Id,
    Guid DoctorId,
    string Date,
    string StartTime,
    int DurationMinutes,
    string VisitKind,
    DateTime StartsAtUtc,
    bool Booked);

public sealed record CreateAvailabilityRequest(
    string Date,
    string StartTime,
    string? EndTime,
    int DurationMinutes,
    string VisitKind);
