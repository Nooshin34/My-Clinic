using System.Text.RegularExpressions;
using Clinic.Application.Common;
using Clinic.Application.Dtos;
using Clinic.Application.Interfaces;
using Clinic.Domain.Entities;

namespace Clinic.Application.Services;

public sealed class DoctorService(IDoctorRepository doctors, IAppointmentRepository appointments, IUnitOfWork unitOfWork) : IDoctorService
{
    private static readonly Regex EmailPattern = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.CultureInvariant | RegexOptions.Compiled);

    public async Task<IReadOnlyList<DoctorDto>> GetAllAsync(string? search, CancellationToken cancellationToken)
    {
        var items = await doctors.ListAsync(search, cancellationToken);
        return items.Select(ToDto).ToList();
    }

    public async Task<DoctorDto> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var doctor = await doctors.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("پزشک پیدا نشد.");

        return ToDto(doctor);
    }

    public async Task<DoctorDto> CreateAsync(UpsertDoctorRequest request, CancellationToken cancellationToken)
    {
        await ValidateAsync(request, null, cancellationToken);
        var doctor = new Doctor();
        Apply(doctor, request);
        await doctors.AddAsync(doctor, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ToDto(doctor);
    }

    public async Task<DoctorDto> UpdateAsync(Guid id, UpsertDoctorRequest request, CancellationToken cancellationToken)
    {
        var doctor = await doctors.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("پزشک پیدا نشد.");

        await ValidateAsync(request, id, cancellationToken);
        Apply(doctor, request);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ToDto(doctor);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var doctor = await doctors.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("پزشک پیدا نشد.");

        if (await appointments.AnyForDoctorAsync(id, cancellationToken))
            throw new AppValidationException(["این پزشک نوبت دارد و نمی‌توان حذفش کرد."]);

        doctors.Remove(doctor);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task ValidateAsync(UpsertDoctorRequest request, Guid? ignoreId, CancellationToken cancellationToken)
    {
        var errors = new List<string>();
        Require(errors, request.FirstName, "نام را وارد کنید.", 80, "نام طولانی است.");
        Require(errors, request.LastName, "نام خانوادگی را وارد کنید.", 80, "نام خانوادگی طولانی است.");
        Require(errors, request.Specialty, "تخصص را وارد کنید.", 80, "تخصص طولانی است.");
        Require(errors, request.LicenseNumber, "شماره نظام پزشکی را وارد کنید.", 32, "شماره نظام پزشکی طولانی است.");

        var phone = request.Phone?.Trim() ?? string.Empty;
        var digits = new string(phone.Where(char.IsDigit).ToArray());
        if (digits.Length is < 8 or > 15)
            errors.Add("شماره تلفن معتبر نیست.");

        var email = request.Email?.Trim() ?? string.Empty;
        if (!EmailPattern.IsMatch(email) || email.Length > 120)
            errors.Add("ایمیل معتبر نیست.");

        if (errors.Count > 0)
            throw new AppValidationException(errors);

        var license = request.LicenseNumber.Trim();
        if (await doctors.LicenseExistsAsync(license, ignoreId, cancellationToken))
            errors.Add("این شماره نظام پزشکی قبلاً ثبت شده است.");

        if (await doctors.EmailExistsAsync(email, ignoreId, cancellationToken))
            errors.Add("این ایمیل قبلاً ثبت شده است.");

        if (errors.Count > 0)
            throw new AppValidationException(errors);
    }

    private static void Apply(Doctor doctor, UpsertDoctorRequest request)
    {
        doctor.FirstName = request.FirstName.Trim();
        doctor.LastName = request.LastName.Trim();
        doctor.Specialty = request.Specialty.Trim();
        doctor.Phone = request.Phone.Trim();
        doctor.Email = request.Email.Trim();
        doctor.LicenseNumber = request.LicenseNumber.Trim();
    }

    private static void Require(List<string> errors, string? value, string requiredMessage, int maxLength, string lengthMessage)
    {
        if (string.IsNullOrWhiteSpace(value))
            errors.Add(requiredMessage);
        else if (value.Trim().Length > maxLength)
            errors.Add(lengthMessage);
    }

    private static DoctorDto ToDto(Doctor doctor) => new(
        doctor.Id,
        doctor.FirstName,
        doctor.LastName,
        doctor.Specialty,
        doctor.Phone,
        doctor.Email,
        doctor.LicenseNumber);
}
