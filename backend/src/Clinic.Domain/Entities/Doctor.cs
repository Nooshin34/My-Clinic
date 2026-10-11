using Clinic.Domain.Common;

namespace Clinic.Domain.Entities;

public class Doctor : Entity
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public Guid SpecialtyId { get; set; }

    public Specialty Specialty { get; set; } = null!;

    public string Phone { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string LicenseNumber { get; set; } = string.Empty;

    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
