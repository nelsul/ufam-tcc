using IcompCare.Application.DTOs;
using IcompCare.Application.DTOs.Subjects;
using IcompCare.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IcompCare.Api.Controllers;

[Authorize(Roles = "Admin,Assistant,Professional,Professor")]
[ApiController]
[Route("api/[controller]")]
public class SubjectsController : ControllerBase
{
    private readonly ISubjectService _subjectService;

    public SubjectsController(ISubjectService subjectService)
    {
        _subjectService = subjectService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<SubjectDto>>> GetSubjects(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool includeInactive = false,
        [FromQuery] string? search = null
    )
    {
        var subjects = await _subjectService.GetAllSubjectsAsync(
            pageNumber,
            pageSize,
            includeInactive,
            search
        );
        return Ok(subjects);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SubjectDto>> GetSubject(Guid id)
    {
        var subject = await _subjectService.GetSubjectByIdAsync(id);
        if (subject == null)
        {
            return NotFound();
        }
        return Ok(subject);
    }

    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<SubjectDto>>> GetActiveSubjects(
        [FromQuery] string? search = null
    )
    {
        var subjects = await _subjectService.GetActiveSubjectsAsync(search);
        return Ok(subjects);
    }

    [HttpPost]
    public async Task<ActionResult<SubjectDto>> CreateSubject(CreateSubjectDto createSubjectDto)
    {
        var createdSubject = await _subjectService.CreateSubjectAsync(createSubjectDto);
        return CreatedAtAction(
            nameof(GetSubject),
            new { id = createdSubject.PublicId },
            createdSubject
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSubject(Guid id, UpdateSubjectDto updateSubjectDto)
    {
        await _subjectService.UpdateSubjectAsync(id, updateSubjectDto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSubject(Guid id)
    {
        await _subjectService.DeleteSubjectAsync(id);
        return NoContent();
    }
}
