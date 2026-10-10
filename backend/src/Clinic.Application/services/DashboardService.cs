using Clinic.Application.Common;
using Clinic.Application.Dtos;
using Clinic.Application.Interfaces;
using Clinic.Domain.Enums;

namespace Clinic.Application.Services;

public sealed class DashboardService(
    IPatientRepository patients,
    IDoctorRepository doctors,
    IAppointmentRepository appointments) : IDashboardService
{
    public async Task<DashboardDto> GetAsync(CancellationToken cancellationToken)
    {
        var (startUtc, endUtc) = ClinicClock.TodayUtc();
        var today = await appointments.ListBetweenAsync(startUtc, endUtc, cancellationToken);

        return new DashboardDto(
            await patients.CountAsync(cancellationToken),
            await doctors.CountAsync(cancellationToken),
            today.Count,
            await appointments.CountByStatusAsync(AppointmentStatus.Scheduled, cancellationToken),
            today.Select(AppointmentMapper.ToDto).ToList());
    }
}
