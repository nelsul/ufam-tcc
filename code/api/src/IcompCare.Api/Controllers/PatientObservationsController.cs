using System.Security.Claims;
using IcompCare.Application.DTOs;
using IcompCare.Application.DTOs.PatientObservations;
using IcompCare.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IcompCare.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientObservationsController : ControllerBase
{
    private readonly IPatientObservationService _patientObservationService;

    public PatientObservationsController(IPatientObservationService patientObservationService)
    {
        _patientObservationService = patientObservationService;
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

    [HttpGet]
    public async Task<ActionResult<PagedResult<PatientObservationDto>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10
    )
    {
        var result = await _patientObservationService.GetAllAsync(pageNumber, pageSize);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PatientObservationDto>> GetById(Guid id)
    {
        var result = await _patientObservationService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpGet("student/{studentId}")]
    public async Task<ActionResult<IEnumerable<PatientObservationDto>>> GetByStudentId(
        Guid studentId
    )
    {
        var result = await _patientObservationService.GetByStudentIdAsync(studentId);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<PatientObservationDto>> Create(CreatePatientObservationDto dto)
    {
        dto.ProfessionalId = GetCurrentUserId();
        var result = await _patientObservationService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<PatientObservationDto>> Update(
        Guid id,
        UpdatePatientObservationDto dto
    )
    {
        var result = await _patientObservationService.UpdateAsync(id, dto);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _patientObservationService.DeleteAsync(id);
        return NoContent();
    }
}
