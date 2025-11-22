using IcompCare.Application.DTOs;
using IcompCare.Application.DTOs.SessionTypes;
using IcompCare.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IcompCare.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SessionTypesController : ControllerBase
{
    private readonly ISessionTypeService _sessionTypeService;

    public SessionTypesController(ISessionTypeService sessionTypeService)
    {
        _sessionTypeService = sessionTypeService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<SessionTypeDto>>> GetSessionTypes(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool includeInactive = false,
        [FromQuery] string? search = null
    )
    {
        var sessionTypes = await _sessionTypeService.GetAllSessionTypesAsync(
            pageNumber,
            pageSize,
            includeInactive,
            search
        );
        return Ok(sessionTypes);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SessionTypeDto>> GetSessionType(Guid id)
    {
        var sessionType = await _sessionTypeService.GetSessionTypeByIdAsync(id);
        if (sessionType == null)
        {
            return NotFound();
        }
        return Ok(sessionType);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Assistant,Professional")]
    public async Task<ActionResult<SessionTypeDto>> CreateSessionType(
        CreateSessionTypeDto createSessionTypeDto
    )
    {
        var createdSessionType = await _sessionTypeService.CreateSessionTypeAsync(
            createSessionTypeDto
        );
        return CreatedAtAction(
            nameof(GetSessionType),
            new { id = createdSessionType.PublicId },
            createdSessionType
        );
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Assistant,Professional")]
    public async Task<IActionResult> UpdateSessionType(
        Guid id,
        UpdateSessionTypeDto updateSessionTypeDto
    )
    {
        await _sessionTypeService.UpdateSessionTypeAsync(id, updateSessionTypeDto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Assistant,Professional")]
    public async Task<IActionResult> DeleteSessionType(Guid id)
    {
        await _sessionTypeService.DeleteSessionTypeAsync(id);
        return NoContent();
    }
}
