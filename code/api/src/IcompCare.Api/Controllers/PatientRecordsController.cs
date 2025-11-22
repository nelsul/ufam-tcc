using IcompCare.Application.DTOs;
using IcompCare.Application.DTOs.PatientRecords;
using IcompCare.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IcompCare.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientRecordsController : ControllerBase
{
    private readonly IPatientRecordService _patientRecordService;

    public PatientRecordsController(IPatientRecordService patientRecordService)
    {
        _patientRecordService = patientRecordService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<PatientRecordDto>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10
    )
    {
        var result = await _patientRecordService.GetAllAsync(pageNumber, pageSize);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PatientRecordDto>> GetById(Guid id)
    {
        var result = await _patientRecordService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpGet("student/{studentId}")]
    public async Task<ActionResult<IEnumerable<PatientRecordDto>>> GetByStudentId(Guid studentId)
    {
        var result = await _patientRecordService.GetByStudentIdAsync(studentId);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<PatientRecordDto>> Create(CreatePatientRecordDto dto)
    {
        var result = await _patientRecordService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<PatientRecordDto>> Update(Guid id, UpdatePatientRecordDto dto)
    {
        var result = await _patientRecordService.UpdateAsync(id, dto);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _patientRecordService.DeleteAsync(id);
        return NoContent();
    }
}
