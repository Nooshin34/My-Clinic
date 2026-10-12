using Clinic.Application.Dtos;

namespace Clinic.Application.Interfaces;

public interface IAvailabilityService
{
    Task<IReadOnlyList<AvailabilitySlotDto>> ListAsync(Guid doctorId, string? from, string? to, CancellationToken cancellationToken);

    Task<IReadOnlyList<AvailabilitySlotDto>> CreateAsync(Guid doctorId, CreateAvailabilityRequest request, CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
