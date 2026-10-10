using Clinic.Application.Dtos;

namespace Clinic.Application.Interfaces;

public interface IAppointmentService
{
    Task<IReadOnlyList<AppointmentDto>> GetAllAsync(string? search, CancellationToken cancellationToken);

    Task<AppointmentDto> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<AppointmentDto> CreateAsync(UpsertAppointmentRequest request, CancellationToken cancellationToken);

    Task<AppointmentDto> UpdateAsync(Guid id, UpsertAppointmentRequest request, CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
