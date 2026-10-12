using Clinic.Application.Dtos;
using Clinic.Domain.Entities;

namespace Clinic.Application.Services;

public static class AppointmentMapper
{
    public static AppointmentDto ToDto(Appointment appointment) => new(
        appointment.Id,
        appointment.PatientId,
        $"{appointment.Patient.FirstName} {appointment.Patient.LastName}",
        appointment.DoctorId,
        $"{appointment.Doctor.FirstName} {appointment.Doctor.LastName}",
        appointment.Doctor.Specialty.Name,
        DateTime.SpecifyKind(appointment.ScheduledAtUtc, DateTimeKind.Utc),
        appointment.Reason,
        appointment.Status.ToString(),
        appointment.VisitKind.ToString(),
        appointment.Notes);
}
