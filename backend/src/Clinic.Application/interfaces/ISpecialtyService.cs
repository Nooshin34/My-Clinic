using Clinic.Application.Dtos;

namespace Clinic.Application.Interfaces;

public interface ISpecialtyService
{
    Task<IReadOnlyList<SpecialtyDto>> GetAllAsync(CancellationToken cancellationToken);

    Task<SpecialtyDto> CreateAsync(CreateSpecialtyRequest request, CancellationToken cancellationToken);
}
