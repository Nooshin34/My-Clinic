using Clinic.Application.Common;
using Clinic.Application.Dtos;
using Clinic.Application.Interfaces;
using Clinic.Domain.Entities;

namespace Clinic.Application.Services;

public sealed class SpecialtyService(ISpecialtyRepository specialties, IUnitOfWork unitOfWork) : ISpecialtyService
{
    public async Task<IReadOnlyList<SpecialtyDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var items = await specialties.ListAsync(cancellationToken);
        return items.Select(ToDto).ToList();
    }

    public async Task<SpecialtyDto> CreateAsync(CreateSpecialtyRequest request, CancellationToken cancellationToken)
    {
        var name = Normalize(request.Name);
        if (name.Length == 0)
            throw new AppValidationException(["نام تخصص را وارد کنید."]);

        if (name.Length > 80)
            throw new AppValidationException(["نام تخصص طولانی است."]);

        var existing = await specialties.FindByNameAsync(name, cancellationToken);
        if (existing is not null)
            return ToDto(existing);

        var specialty = new Specialty { Name = name };
        await specialties.AddAsync(specialty, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ToDto(specialty);
    }

    private static SpecialtyDto ToDto(Specialty specialty) => new(specialty.Id, specialty.Name);

    private static string Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        return string.Join(' ', value.Trim().Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
    }
}
