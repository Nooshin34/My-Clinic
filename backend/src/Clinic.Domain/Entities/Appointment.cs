using Clinic.Domain.Common;
using Clinic.Domain.Enums;

namespace Clinic.Domain.Entities;

public class Appointment : Entity
{
    public Guid PatientId { get; set; }

    public Patient Patient { get; set; } = null!;

    public Guid DoctorId { get; set; }

    public Doctor Doctor { get; set; } = null!;

    public DateTime ScheduledAtUtc { get; set; }

    public string Reason { get; set; } = string.Empty;

    public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;

    public VisitKind VisitKind { get; set; } = VisitKind.InPerson;

    public string? Notes { get; set; }
}
