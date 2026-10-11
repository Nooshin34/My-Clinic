namespace Clinic.Application.Dtos;

public sealed record DoctorDto(
    Guid Id,
    string FirstName,
    string LastName,
    Guid SpecialtyId,
    string Specialty,
    string Phone,
    string Email,
    string LicenseNumber);
