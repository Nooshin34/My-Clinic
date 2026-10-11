using Clinic.Application.Dtos;

namespace Clinic.Application.Interfaces;

public interface IDoctorService
{
    Task<PagedResult<DoctorDto>> GetAllAsync(string? search, int page, int pageSize, CancellationToken cancellationToken);

    Task<DoctorDto> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<DoctorDto> CreateAsync(UpsertDoctorRequest request, CancellationToken cancellationToken);

    Task<DoctorDto> UpdateAsync(Guid id, UpsertDoctorRequest request, CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
