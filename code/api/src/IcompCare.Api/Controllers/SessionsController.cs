using System.Security.Claims;
using IcompCare.Application.DTOs;
using IcompCare.Application.DTOs.Sessions;
using IcompCare.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IcompCare.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SessionsController : ControllerBase
{
    private readonly ISessionService _sessionService;

    public SessionsController(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim =
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<SessionDto>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10
    )
    {
        var result = await _sessionService.GetAllAsync(pageNumber, pageSize);
        return Ok(result);
    }

    [HttpGet("my")]
    public async Task<ActionResult<PagedResult<SessionDto>>> GetMySessions(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] DateTimeOffset? dateFrom = null,
        [FromQuery] DateTimeOffset? dateTo = null,
        [FromQuery] string? search = null
    )
    {
        if (dateFrom.HasValue && dateTo.HasValue)
        {
            var maxRange = TimeSpan.FromDays(14);
            if (dateTo.Value - dateFrom.Value > maxRange)
            {
                return BadRequest("Date range cannot exceed 2 weeks.");
            }
        }

        var professionalId = GetCurrentUserId();
        var result = await _sessionService.GetByProfessionalAsync(
            professionalId,
            pageNumber,
            pageSize,
            dateFrom,
            dateTo,
            search
        );
        return Ok(result);
    }

    [HttpGet("my/open")]
    public async Task<ActionResult<SessionDto?>> GetMyOpenSession()
    {
        var professionalId = GetCurrentUserId();
        var result = await _sessionService.GetOpenSessionByProfessionalAsync(professionalId);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SessionDto>> GetById(Guid id)
    {
        var result = await _sessionService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<SessionDto>> Create(CreateSessionDto dto)
    {
        var result = await _sessionService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPost("start/{appointmentId}")]
    public async Task<ActionResult<SessionDto>> StartSession(Guid appointmentId)
    {
        var professionalId = GetCurrentUserId();
        var result = await _sessionService.StartSessionAsync(appointmentId, professionalId);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<SessionDto>> Update(Guid id, UpdateSessionDto dto)
    {
        var result = await _sessionService.UpdateAsync(id, dto);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _sessionService.DeleteAsync(id);
        return NoContent();
    }
}
