using Clinic.Application.Dtos;
using Clinic.Domain.Entities;

namespace Clinic.Application.Interfaces;

public interface IDoctorRepository
{
    Task<PagedResult<Doctor>> ListAsync(string? search, int page, int pageSize, CancellationToken cancellationToken);

    Task<Doctor?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<int> CountAsync(CancellationToken cancellationToken);

    Task<bool> LicenseExistsAsync(string licenseNumber, Guid? ignoreId, CancellationToken cancellationToken);

    Task<bool> EmailExistsAsync(string email, Guid? ignoreId, CancellationToken cancellationToken);

    Task AddAsync(Doctor doctor, CancellationToken cancellationToken);

    void Remove(Doctor doctor);
}
