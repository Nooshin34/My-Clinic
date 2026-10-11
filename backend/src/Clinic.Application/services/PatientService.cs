using Clinic.Application.Common;
using Clinic.Application.Dtos;
using Clinic.Application.Interfaces;
using Clinic.Domain.Entities;
using Clinic.Domain.Enums;

namespace Clinic.Application.Services;

public sealed class PatientService(IPatientRepository patients, IAppointmentRepository appointments, IUnitOfWork unitOfWork) : IPatientService
{
    public async Task<PagedResult<PatientDto>> GetAllAsync(string? search, int page, int pageSize, CancellationToken cancellationToken)
    {
        var (currentPage, size) = PageQuery.Normalize(page, pageSize);
        var result = await patients.ListAsync(search, currentPage, size, cancellationToken);
        return new PagedResult<PatientDto>(result.Items.Select(ToDto).ToList(), result.Page, result.PageSize, result.TotalCount);
    }

    public async Task<PatientDto> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var patient = await patients.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("بیمار پیدا نشد.");

        return ToDto(patient);
    }

    public async Task<PatientDto> CreateAsync(UpsertPatientRequest request, CancellationToken cancellationToken)
    {
        var gender = await ValidateAsync(request, null, cancellationToken);
        var patient = new Patient();
        Apply(patient, request, gender);
        await patients.AddAsync(patient, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ToDto(patient);
    }

    public async Task<PatientDto> UpdateAsync(Guid id, UpsertPatientRequest request, CancellationToken cancellationToken)
    {
        var patient = await patients.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("بیمار پیدا نشد.");

        var gender = await ValidateAsync(request, id, cancellationToken);
        Apply(patient, request, gender);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ToDto(patient);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var patient = await patients.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("بیمار پیدا نشد.");

        if (await appointments.AnyForPatientAsync(id, cancellationToken))
            throw new AppValidationException(["این بیمار نوبت دارد و نمی‌توان حذفش کرد."]);

        patients.Remove(patient);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<Gender> ValidateAsync(UpsertPatientRequest request, Guid? ignoreId, CancellationToken cancellationToken)
    {
        var errors = new List<string>();
        Require(errors, request.FirstName, "نام را وارد کنید.", 80, "نام طولانی است.");
        Require(errors, request.LastName, "نام خانوادگی را وارد کنید.", 80, "نام خانوادگی طولانی است.");

        var nationalId = request.NationalId?.Trim() ?? string.Empty;
        if (nationalId.Length != 10 || nationalId.Any(character => !char.IsDigit(character)))
            errors.Add("کد ملی باید ۱۰ رقم باشد.");

        var phone = request.Phone?.Trim() ?? string.Empty;
        var digits = new string(phone.Where(char.IsDigit).ToArray());
        if (digits.Length is < 8 or > 15)
            errors.Add("شماره تلفن معتبر نیست.");

        if (request.DateOfBirth == default)
            errors.Add("تاریخ تولد را وارد کنید.");
        else if (request.DateOfBirth > DateOnly.FromDateTime(DateTime.Today))
            errors.Add("تاریخ تولد نمی‌تواند در آینده باشد.");
        else if (request.DateOfBirth < DateOnly.FromDateTime(DateTime.Today.AddYears(-120)))
            errors.Add("تاریخ تولد معتبر نیست.");

        if (!string.IsNullOrWhiteSpace(request.Address) && request.Address.Trim().Length > 300)
            errors.Add("آدرس طولانی است.");

        var gender = ClinicEnums.ParseGender(request.Gender);
        if (gender is null)
            errors.Add("جنسیت معتبر نیست.");

        if (errors.Count > 0)
            throw new AppValidationException(errors);

        if (await patients.NationalIdExistsAsync(nationalId, ignoreId, cancellationToken))
            throw new AppValidationException(["این کد ملی قبلاً ثبت شده است."]);

        return gender!.Value;
    }

    private static void Apply(Patient patient, UpsertPatientRequest request, Gender gender)
    {
        patient.FirstName = request.FirstName.Trim();
        patient.LastName = request.LastName.Trim();
        patient.NationalId = request.NationalId.Trim();
        patient.Phone = request.Phone.Trim();
        patient.DateOfBirth = request.DateOfBirth;
        patient.Gender = gender;
        patient.Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim();
    }

    private static void Require(List<string> errors, string? value, string requiredMessage, int maxLength, string lengthMessage)
    {
        if (string.IsNullOrWhiteSpace(value))
            errors.Add(requiredMessage);
        else if (value.Trim().Length > maxLength)
            errors.Add(lengthMessage);
    }

    private static PatientDto ToDto(Patient patient) => new(
        patient.Id,
        patient.FirstName,
        patient.LastName,
        patient.NationalId,
        patient.Phone,
        patient.DateOfBirth,
        patient.Gender.ToString(),
        patient.Address);
}
