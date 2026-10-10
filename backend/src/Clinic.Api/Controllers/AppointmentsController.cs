using Clinic.Application.Dtos;
using Clinic.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.Api.Controllers;

[ApiController]
[Route("api/appointments")]
public class AppointmentsController(IAppointmentService appointments) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AppointmentDto>>> GetAll([FromQuery] string? search, CancellationToken cancellationToken) =>
        Ok(await appointments.GetAllAsync(search, cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AppointmentDto>> GetById(Guid id, CancellationToken cancellationToken) =>
        Ok(await appointments.GetByIdAsync(id, cancellationToken));

    [HttpPost]
    public async Task<ActionResult<AppointmentDto>> Create(UpsertAppointmentRequest request, CancellationToken cancellationToken)
    {
        var created = await appointments.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AppointmentDto>> Update(Guid id, UpsertAppointmentRequest request, CancellationToken cancellationToken) =>
        Ok(await appointments.UpdateAsync(id, request, cancellationToken));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await appointments.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
