using IcompCare.Application.DTOs;
using IcompCare.Application.DTOs.Semesters;
using IcompCare.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IcompCare.Api.Controllers;

[Authorize(Roles = "Admin,Assistant,Professional,Professor")]
[ApiController]
[Route("api/[controller]")]
public class SemestersController : ControllerBase
{
    private readonly ISemesterService _semesterService;

    public SemestersController(ISemesterService semesterService)
    {
        _semesterService = semesterService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<SemesterDto>>> GetSemesters(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool includeInactive = false,
        [FromQuery] string? search = null
    )
    {
        var semesters = await _semesterService.GetAllSemestersAsync(
            pageNumber,
            pageSize,
            includeInactive,
            search
        );
        return Ok(semesters);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SemesterDto>> GetSemester(Guid id)
    {
        var semester = await _semesterService.GetSemesterByIdAsync(id);
        if (semester == null)
        {
            return NotFound();
        }
        return Ok(semester);
    }

    [HttpGet("current")]
    public async Task<ActionResult<SemesterDto>> GetCurrentSemester()
    {
        var semester = await _semesterService.GetCurrentSemesterAsync();
        if (semester == null)
        {
            return NotFound();
        }
        return Ok(semester);
    }

    [HttpPost]
    public async Task<ActionResult<SemesterDto>> CreateSemester(CreateSemesterDto createSemesterDto)
    {
        var createdSemester = await _semesterService.CreateSemesterAsync(createSemesterDto);
        return CreatedAtAction(
            nameof(GetSemester),
            new { id = createdSemester.PublicId },
            createdSemester
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSemester(Guid id, UpdateSemesterDto updateSemesterDto)
    {
        await _semesterService.UpdateSemesterAsync(id, updateSemesterDto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSemester(Guid id)
    {
        await _semesterService.DeleteSemesterAsync(id);
        return NoContent();
    }
}
