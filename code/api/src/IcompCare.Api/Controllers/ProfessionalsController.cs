using System.Security.Claims;
using IcompCare.Application.DTOs;
using IcompCare.Application.DTOs.Appointments;
using IcompCare.Application.DTOs.Availabilities;
using IcompCare.Application.DTOs.Users;
using IcompCare.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IcompCare.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProfessionalsController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IAvailabilityService _availabilityService;
    private readonly IAppointmentService _appointmentService;

    public ProfessionalsController(
        IUserService userService,
        IAvailabilityService availabilityService,
        IAppointmentService appointmentService
    )
    {
        _userService = userService;
        _availabilityService = availabilityService;
        _appointmentService = appointmentService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetActiveProfessionals()
    {
        var professionals = await _userService.GetActiveProfessionalsAsync();
        return Ok(professionals);
    }

    [HttpGet("{id}/availability")]
    public async Task<
        ActionResult<IEnumerable<ProfessionalAvailabilityDto>>
    > GetProfessionalAvailability(Guid id)
    {
        var availability = await _availabilityService.GetProfessionalAvailabilityAsync(id);
        return Ok(availability);
    }

    [HttpGet("me/availabilities")]
    [Authorize(Roles = "Professional, Admin")]
    public async Task<ActionResult<IEnumerable<AvailabilityDto>>> GetMyAvailabilities()
    {
        var userId = GetCurrentUserId();
        var availabilities = await _availabilityService.GetMyAvailabilitiesAsync(userId);
        return Ok(availabilities);
    }

    [HttpPost("me/availabilities")]
    [Authorize(Roles = "Professional, Admin")]
    public async Task<ActionResult<AvailabilityDto>> CreateMyAvailability(
        CreateMyAvailabilityDto dto
    )
    {
        var userId = GetCurrentUserId();
        var created = await _availabilityService.CreateMyAvailabilityAsync(userId, dto);
        return CreatedAtAction(nameof(GetMyAvailabilities), new { }, created);
    }

    [HttpPut("me/availabilities/{id}")]
    [Authorize(Roles = "Professional, Admin")]
    public async Task<IActionResult> UpdateMyAvailability(Guid id, UpdateAvailabilityDto dto)
    {
        var userId = GetCurrentUserId();
        await _availabilityService.UpdateMyAvailabilityAsync(userId, id, dto);
        return NoContent();
    }

    [HttpDelete("me/availabilities/{id}")]
    [Authorize(Roles = "Professional, Admin")]
    public async Task<IActionResult> DeleteMyAvailability(Guid id)
    {
        var userId = GetCurrentUserId();
        await _availabilityService.DeleteMyAvailabilityAsync(userId, id);
        return NoContent();
    }

    [HttpGet("me/appointments")]
    [Authorize(Roles = "Professional, Admin")]
    public async Task<ActionResult<PagedResult<AppointmentDto>>> GetMyAppointments(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null,
        [FromQuery] Guid? sessionTypeId = null,
        [FromQuery] string? search = null
    )
    {
        var userId = GetCurrentUserId();
        var appointments = await _appointmentService.GetByProfessionalIdAsync(
            userId,
            pageNumber,
            pageSize,
            status,
            sessionTypeId,
            search
        );
        return Ok(appointments);
    }

    private Guid GetCurrentUserId()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (idClaim == null || !Guid.TryParse(idClaim.Value, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user token.");
        }
        return userId;
    }
}
