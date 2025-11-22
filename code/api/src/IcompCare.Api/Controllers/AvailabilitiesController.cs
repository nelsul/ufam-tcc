using IcompCare.Application.DTOs;
using IcompCare.Application.DTOs.Availabilities;
using IcompCare.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IcompCare.Api.Controllers;

[Authorize(Roles = "Admin,Assistant,Professional")]
[ApiController]
[Route("api/[controller]")]
public class AvailabilitiesController : ControllerBase
{
    private readonly IAvailabilityService _availabilityService;

    public AvailabilitiesController(IAvailabilityService availabilityService)
    {
        _availabilityService = availabilityService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<AvailabilityDto>>> GetAvailabilities(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool includeInactive = false
    )
    {
        var availabilities = await _availabilityService.GetAllAvailabilitiesAsync(
            pageNumber,
            pageSize,
            includeInactive
        );
        return Ok(availabilities);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AvailabilityDto>> GetAvailability(Guid id)
    {
        var availability = await _availabilityService.GetAvailabilityByIdAsync(id);
        if (availability == null)
        {
            return NotFound();
        }
        return Ok(availability);
    }

    [HttpPost]
    public async Task<ActionResult<AvailabilityDto>> CreateAvailability(
        CreateAvailabilityDto createAvailabilityDto
    )
    {
        var createdAvailability = await _availabilityService.CreateAvailabilityAsync(
            createAvailabilityDto
        );
        return CreatedAtAction(
            nameof(GetAvailability),
            new { id = createdAvailability.PublicId },
            createdAvailability
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAvailability(
        Guid id,
        UpdateAvailabilityDto updateAvailabilityDto
    )
    {
        await _availabilityService.UpdateAvailabilityAsync(id, updateAvailabilityDto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAvailability(Guid id)
    {
        await _availabilityService.DeleteAvailabilityAsync(id);
        return NoContent();
    }
}
