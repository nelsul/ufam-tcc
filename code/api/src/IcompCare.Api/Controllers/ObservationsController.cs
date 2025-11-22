using IcompCare.Application.DTOs;
using IcompCare.Application.DTOs.Observations;
using IcompCare.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IcompCare.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin,Assistant,Professional")]
[Route("api/[controller]")]
public class ObservationsController : ControllerBase
{
    private readonly IObservationService _observationService;

    public ObservationsController(IObservationService observationService)
    {
        _observationService = observationService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<ObservationDto>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool includeInactive = false,
        [FromQuery] string? search = null
    )
    {
        var result = await _observationService.GetAllAsync(
            pageNumber,
            pageSize,
            includeInactive,
            search
        );
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ObservationDto>> GetById(Guid id)
    {
        var result = await _observationService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ObservationDto>> Create(CreateObservationDto dto)
    {
        var result = await _observationService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ObservationDto>> Update(Guid id, UpdateObservationDto dto)
    {
        var result = await _observationService.UpdateAsync(id, dto);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _observationService.DeleteAsync(id);
        return NoContent();
    }
}
