using Clinic.Application.Common;
using Clinic.Application.Dtos;
using Clinic.Application.Interfaces;
using Clinic.Domain.Entities;
using Clinic.Domain.Enums;

namespace Clinic.Application.Services;

public sealed class AppointmentService(
    IAppointmentRepository appointments,
    IPatientRepository patients,
    IDoctorRepository doctors,
    IUnitOfWork unitOfWork) : IAppointmentService
{
    public async Task<IReadOnlyList<AppointmentDto>> GetAllAsync(string? search, CancellationToken cancellationToken)
    {
        var items = await appointments.ListAsync(search, cancellationToken);
        return items.Select(AppointmentMapper.ToDto).ToList();
    }

    public async Task<AppointmentDto> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var appointment = await appointments.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("نوبت پیدا نشد.");

        return AppointmentMapper.ToDto(appointment);
    }

    public async Task<AppointmentDto> CreateAsync(UpsertAppointmentRequest request, CancellationToken cancellationToken)
    {
        var (patient, doctor, scheduledAt, status) = await ValidateAsync(request, null, cancellationToken);
        var appointment = new Appointment();
        Apply(appointment, request, patient, doctor, scheduledAt, status);
        await appointments.AddAsync(appointment, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return AppointmentMapper.ToDto(appointment);
    }

    public async Task<AppointmentDto> UpdateAsync(Guid id, UpsertAppointmentRequest request, CancellationToken cancellationToken)
    {
        var appointment = await appointments.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("نوبت پیدا نشد.");

        var (patient, doctor, scheduledAt, status) = await ValidateAsync(request, id, cancellationToken);
        Apply(appointment, request, patient, doctor, scheduledAt, status);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return AppointmentMapper.ToDto(appointment);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var appointment = await appointments.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("نوبت پیدا نشد.");

        appointments.Remove(appointment);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<(Patient Patient, Doctor Doctor, DateTime ScheduledAt, AppointmentStatus Status)> ValidateAsync(
        UpsertAppointmentRequest request,
        Guid? ignoreId,
        CancellationToken cancellationToken)
    {
        var errors = new List<string>();

        if (request.PatientId == Guid.Empty)
            errors.Add("بیمار را انتخاب کنید.");

        if (request.DoctorId == Guid.Empty)
            errors.Add("پزشک را انتخاب کنید.");

        var scheduledAt = NormalizeUtc(request.ScheduledAtUtc);
        if (scheduledAt.Year is < 2000 or > 2100)
            errors.Add("زمان نوبت معتبر نیست.");

        Require(errors, request.Reason, "دلیل مراجعه را وارد کنید.", 200, "دلیل مراجعه طولانی است.");

        if (!string.IsNullOrWhiteSpace(request.Notes) && request.Notes.Trim().Length > 1000)
            errors.Add("یادداشت طولانی است.");

        var status = ClinicEnums.ParseStatus(request.Status);
        if (status is null)
            errors.Add("وضعیت نوبت معتبر نیست.");

        if (errors.Count > 0)
            throw new AppValidationException(errors);

        var patient = await patients.GetByIdAsync(request.PatientId, cancellationToken);
        if (patient is null)
            errors.Add("بیمار انتخاب‌شده وجود ندارد.");

        var doctor = await doctors.GetByIdAsync(request.DoctorId, cancellationToken);
        if (doctor is null)
            errors.Add("پزشک انتخاب‌شده وجود ندارد.");

        if (errors.Count > 0)
            throw new AppValidationException(errors);

        if (status != AppointmentStatus.Cancelled &&
            await appointments.ExistsForDoctorAtAsync(request.DoctorId, scheduledAt, ignoreId, cancellationToken))
        {
            throw new AppValidationException(["این پزشک در همین زمان نوبت دیگری دارد."]);
        }

        return (patient!, doctor!, scheduledAt, status!.Value);
    }

    private static void Apply(
        Appointment appointment,
        UpsertAppointmentRequest request,
        Patient patient,
        Doctor doctor,
        DateTime scheduledAt,
        AppointmentStatus status)
    {
        appointment.PatientId = patient.Id;
        appointment.Patient = patient;
        appointment.DoctorId = doctor.Id;
        appointment.Doctor = doctor;
        appointment.ScheduledAtUtc = scheduledAt;
        appointment.Reason = request.Reason.Trim();
        appointment.Status = status;
        appointment.Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim();
    }

    private static DateTime NormalizeUtc(DateTime value)
    {
        var utc = value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };

        return new DateTime(utc.Year, utc.Month, utc.Day, utc.Hour, utc.Minute, 0, DateTimeKind.Utc);
    }

    private static void Require(List<string> errors, string? value, string requiredMessage, int maxLength, string lengthMessage)
    {
        if (string.IsNullOrWhiteSpace(value))
            errors.Add(requiredMessage);
        else if (value.Trim().Length > maxLength)
            errors.Add(lengthMessage);
    }
}
