namespace Clinic.Application.Dtos;

public sealed record UpsertDoctorRequest(
    string FirstName,
    string LastName,
    Guid SpecialtyId,
    string Phone,
    string Email,
    string LicenseNumber);
