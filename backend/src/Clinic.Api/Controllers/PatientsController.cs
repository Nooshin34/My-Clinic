using Clinic.Application.Dtos;
using Clinic.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.Api.Controllers;

[ApiController]
[Route("api/patients")]
public class PatientsController(IPatientService patients) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PatientDto>>> GetAll([FromQuery] string? search, CancellationToken cancellationToken) =>
        Ok(await patients.GetAllAsync(search, cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PatientDto>> GetById(Guid id, CancellationToken cancellationToken) =>
        Ok(await patients.GetByIdAsync(id, cancellationToken));

    [HttpPost]
    public async Task<ActionResult<PatientDto>> Create(UpsertPatientRequest request, CancellationToken cancellationToken)
    {
        var created = await patients.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<PatientDto>> Update(Guid id, UpsertPatientRequest request, CancellationToken cancellationToken) =>
        Ok(await patients.UpdateAsync(id, request, cancellationToken));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await patients.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
