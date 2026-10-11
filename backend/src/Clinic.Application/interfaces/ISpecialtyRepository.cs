using Clinic.Domain.Entities;

namespace Clinic.Application.Interfaces;

public interface ISpecialtyRepository
{
    Task<IReadOnlyList<Specialty>> ListAsync(CancellationToken cancellationToken);

    Task<Specialty?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Specialty?> FindByNameAsync(string name, CancellationToken cancellationToken);

    Task AddAsync(Specialty specialty, CancellationToken cancellationToken);
}
