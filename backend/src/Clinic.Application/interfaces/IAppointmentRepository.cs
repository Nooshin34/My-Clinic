using Clinic.Domain.Entities;
using Clinic.Domain.Enums;

namespace Clinic.Application.Interfaces;

public interface IAppointmentRepository
{
    Task<IReadOnlyList<Appointment>> ListAsync(string? search, CancellationToken cancellationToken);

    Task<IReadOnlyList<Appointment>> ListBetweenAsync(DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken);

    Task<Appointment?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<int> CountByStatusAsync(AppointmentStatus status, CancellationToken cancellationToken);

    Task<bool> AnyForPatientAsync(Guid patientId, CancellationToken cancellationToken);

    Task<bool> AnyForDoctorAsync(Guid doctorId, CancellationToken cancellationToken);

    Task<bool> ExistsForDoctorAtAsync(Guid doctorId, DateTime scheduledAtUtc, Guid? ignoreId, CancellationToken cancellationToken);

    Task AddAsync(Appointment appointment, CancellationToken cancellationToken);

    void Remove(Appointment appointment);
}
