using Clinic.Application.Dtos;
using Clinic.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.Api.Controllers;

[ApiController]
[Route("api/doctors")]
public class DoctorsController(IDoctorService doctors) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<DoctorDto>>> GetAll(
        [FromQuery] string? search,
        [FromQuery] int page,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken) =>
        Ok(await doctors.GetAllAsync(search, page, pageSize, cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DoctorDto>> GetById(Guid id, CancellationToken cancellationToken) =>
        Ok(await doctors.GetByIdAsync(id, cancellationToken));

    [HttpPost]
    public async Task<ActionResult<DoctorDto>> Create(UpsertDoctorRequest request, CancellationToken cancellationToken)
    {
        var created = await doctors.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<DoctorDto>> Update(Guid id, UpsertDoctorRequest request, CancellationToken cancellationToken) =>
        Ok(await doctors.UpdateAsync(id, request, cancellationToken));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await doctors.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
