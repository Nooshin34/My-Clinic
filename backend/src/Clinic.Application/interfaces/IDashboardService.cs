using Clinic.Application.Dtos;

namespace Clinic.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardDto> GetAsync(CancellationToken cancellationToken);
}
