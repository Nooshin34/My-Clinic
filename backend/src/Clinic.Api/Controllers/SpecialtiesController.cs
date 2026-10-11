using Clinic.Application.Dtos;
using Clinic.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.Api.Controllers;

[ApiController]
[Route("api/specialties")]
public class SpecialtiesController(ISpecialtyService specialties) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SpecialtyDto>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await specialties.GetAllAsync(cancellationToken));

    [HttpPost]
    public async Task<ActionResult<SpecialtyDto>> Create(CreateSpecialtyRequest request, CancellationToken cancellationToken)
    {
        var created = await specialties.CreateAsync(request, cancellationToken);
        return Ok(created);
    }
}
