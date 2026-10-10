namespace Clinic.Application.Dtos;

public sealed record DashboardDto(
    int PatientCount,
    int DoctorCount,
    int TodayAppointmentCount,
    int ScheduledAppointmentCount,
    IReadOnlyList<AppointmentDto> TodayAppointments);
