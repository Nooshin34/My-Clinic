using Clinic.Domain.Entities;

namespace Clinic.Application.Interfaces;

public interface IAvailabilityRepository
{
    Task<IReadOnlyList<AvailabilitySlot>> ListAsync(Guid doctorId, DateOnly from, DateOnly to, CancellationToken cancellationToken);

    Task<AvailabilitySlot?> FindAsync(Guid doctorId, DateOnly date, TimeOnly startTime, CancellationToken cancellationToken);

    Task<AvailabilitySlot?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task AddRangeAsync(IEnumerable<AvailabilitySlot> slots, CancellationToken cancellationToken);

    void Remove(AvailabilitySlot slot);
}
