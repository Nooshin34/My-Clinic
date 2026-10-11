namespace Clinic.Application.Dtos;

public sealed record SpecialtyDto(Guid Id, string Name);

public sealed record CreateSpecialtyRequest(string Name);
