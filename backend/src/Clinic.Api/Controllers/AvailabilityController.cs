using Clinic.Application.Dtos;
using Clinic.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.Api.Controllers;

[ApiController]
[Route("api")]
public class AvailabilityController(IAvailabilityService availability) : ControllerBase
{
    [HttpGet("doctors/{doctorId:guid}/availability")]
    public async Task<ActionResult<IReadOnlyList<AvailabilitySlotDto>>> List(
        Guid doctorId,
        [FromQuery] string? from,
        [FromQuery] string? to,
        CancellationToken cancellationToken) =>
        Ok(await availability.ListAsync(doctorId, from, to, cancellationToken));

    [HttpPost("doctors/{doctorId:guid}/availability")]
    public async Task<ActionResult<IReadOnlyList<AvailabilitySlotDto>>> Create(
        Guid doctorId,
        CreateAvailabilityRequest request,
        CancellationToken cancellationToken) =>
        Ok(await availability.CreateAsync(doctorId, request, cancellationToken));

    [HttpDelete("availability/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await availability.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
