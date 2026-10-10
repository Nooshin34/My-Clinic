namespace Clinic.Application.Dtos;

public sealed record UpsertPatientRequest(
    string FirstName,
    string LastName,
    string NationalId,
    string Phone,
    DateOnly DateOfBirth,
    string Gender,
    string? Address);
