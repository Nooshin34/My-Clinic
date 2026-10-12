using Clinic.Domain.Common;
using Clinic.Domain.Enums;

namespace Clinic.Domain.Entities;

public class AvailabilitySlot : Entity
{
    public Guid DoctorId { get; set; }

    public Doctor Doctor { get; set; } = null!;

    public DateOnly Date { get; set; }

    public TimeOnly StartTime { get; set; }

    public int DurationMinutes { get; set; }

    public VisitKind VisitKind { get; set; }
}
