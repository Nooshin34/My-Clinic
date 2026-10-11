using Clinic.Application.Dtos;

namespace Clinic.Application.Interfaces;

public interface IPatientService
{
    Task<PagedResult<PatientDto>> GetAllAsync(string? search, int page, int pageSize, CancellationToken cancellationToken);

    Task<PatientDto> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<PatientDto> CreateAsync(UpsertPatientRequest request, CancellationToken cancellationToken);

    Task<PatientDto> UpdateAsync(Guid id, UpsertPatientRequest request, CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
