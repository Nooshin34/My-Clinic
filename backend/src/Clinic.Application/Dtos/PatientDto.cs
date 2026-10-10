namespace Clinic.Application.Dtos;

public sealed record PatientDto(
    Guid Id,
    string FirstName,
    string LastName,
    string NationalId,
    string Phone,
    DateOnly DateOfBirth,
    string Gender,
    string? Address);
