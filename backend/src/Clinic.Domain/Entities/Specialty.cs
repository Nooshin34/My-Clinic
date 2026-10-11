using Clinic.Domain.Common;

namespace Clinic.Domain.Entities;

public class Specialty : Entity
{
    public string Name { get; set; } = string.Empty;

    public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
}
