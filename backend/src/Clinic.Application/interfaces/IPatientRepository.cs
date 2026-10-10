using Clinic.Domain.Entities;

namespace Clinic.Application.Interfaces;

public interface IPatientRepository
{
    Task<IReadOnlyList<Patient>> ListAsync(string? search, CancellationToken cancellationToken);

    Task<Patient?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<int> CountAsync(CancellationToken cancellationToken);

    Task<bool> NationalIdExistsAsync(string nationalId, Guid? ignoreId, CancellationToken cancellationToken);

    Task AddAsync(Patient patient, CancellationToken cancellationToken);

    void Remove(Patient patient);
}
