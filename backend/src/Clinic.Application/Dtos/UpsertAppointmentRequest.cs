namespace Clinic.Application.Dtos;

public sealed record UpsertAppointmentRequest(
    Guid PatientId,
    Guid DoctorId,
    DateTime ScheduledAtUtc,
    string Reason,
    string Status,
    string? Notes);
