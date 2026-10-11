using Clinic.Application.Dtos;

namespace Clinic.Application.Interfaces;

public interface IAppointmentService
{
    Task<PagedResult<AppointmentDto>> GetAllAsync(string? search, int page, int pageSize, CancellationToken cancellationToken);

    Task<AppointmentDto> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<AppointmentDto> CreateAsync(UpsertAppointmentRequest request, CancellationToken cancellationToken);

    Task<AppointmentDto> UpdateAsync(Guid id, UpsertAppointmentRequest request, CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
