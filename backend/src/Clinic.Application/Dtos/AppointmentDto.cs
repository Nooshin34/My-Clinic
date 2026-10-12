namespace Clinic.Application.Dtos;

public sealed record AppointmentDto(
    Guid Id,
    Guid PatientId,
    string PatientName,
    Guid DoctorId,
    string DoctorName,
    string DoctorSpecialty,
    DateTime ScheduledAtUtc,
    string Reason,
    string Status,
    string VisitKind,
    string? Notes);
