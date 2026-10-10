namespace Clinic.Application.Dtos;

public sealed record UpsertDoctorRequest(
    string FirstName,
    string LastName,
    string Specialty,
    string Phone,
    string Email,
    string LicenseNumber);
