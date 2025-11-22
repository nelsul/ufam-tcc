using System.Security.Claims;
using IcompCare.Application.DTOs;
using IcompCare.Application.DTOs.PatientObservations;
using IcompCare.Application.DTOs.StudentEnrollments;
using IcompCare.Application.DTOs.SubjectOfferings;
using IcompCare.Application.DTOs.Users;
using IcompCare.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IcompCare.Api.Controllers;

[Authorize(Roles = "Admin,Assistant,Professional,Professor,Student")]
[ApiController]
[Route("api/[controller]")]
public class SubjectOfferingsController : ControllerBase
{
    private readonly ISubjectOfferingService _subjectOfferingService;
    private readonly IStudentEnrollmentService _studentEnrollmentService;
    private readonly IUserService _userService;
    private readonly IPatientObservationService _patientObservationService;

    public SubjectOfferingsController(
        ISubjectOfferingService subjectOfferingService,
        IStudentEnrollmentService studentEnrollmentService,
        IUserService userService,
        IPatientObservationService patientObservationService
    )
    {
        _subjectOfferingService = subjectOfferingService;
        _studentEnrollmentService = studentEnrollmentService;
        _userService = userService;
        _patientObservationService = patientObservationService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<SubjectOfferingDto>>> GetSubjectOfferings(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool includeInactive = false,
        [FromQuery] string? search = null
    )
    {
        var subjectOfferings = await _subjectOfferingService.GetAllSubjectOfferingsAsync(
            pageNumber,
            pageSize,
            includeInactive,
            search
        );
        return Ok(subjectOfferings);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SubjectOfferingDto>> GetSubjectOffering(Guid id)
    {
        var subjectOffering = await _subjectOfferingService.GetSubjectOfferingByIdAsync(id);
        if (subjectOffering == null)
        {
            return NotFound();
        }
        return Ok(subjectOffering);
    }

    [HttpGet("active")]
    public async Task<ActionResult<PagedResult<SubjectOfferingDto>>> GetActiveOfferings(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10
    )
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        var offerings = await _subjectOfferingService.GetActiveOfferingsByDateAsync(
            today,
            pageNumber,
            pageSize
        );
        return Ok(offerings);
    }

    [HttpPost]
    public async Task<ActionResult<SubjectOfferingDto>> CreateSubjectOffering(
        CreateSubjectOfferingDto createSubjectOfferingDto
    )
    {
        var createdSubjectOffering = await _subjectOfferingService.CreateSubjectOfferingAsync(
            createSubjectOfferingDto
        );
        return CreatedAtAction(
            nameof(GetSubjectOffering),
            new { id = createdSubjectOffering.PublicId },
            createdSubjectOffering
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSubjectOffering(
        Guid id,
        UpdateSubjectOfferingDto updateSubjectOfferingDto
    )
    {
        await _subjectOfferingService.UpdateSubjectOfferingAsync(id, updateSubjectOfferingDto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSubjectOffering(Guid id)
    {
        await _subjectOfferingService.DeleteSubjectOfferingAsync(id);
        return NoContent();
    }

    [HttpGet("my")]
    [Authorize(Roles = "Professor")]
    public async Task<ActionResult<PagedResult<SubjectOfferingDto>>> GetMyOfferings(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool includeInactive = false,
        [FromQuery] string? search = null
    )
    {
        var professorId = GetCurrentUserId();
        var offerings = await _subjectOfferingService.GetByProfessorIdAsync(
            professorId,
            pageNumber,
            pageSize,
            includeInactive,
            search
        );
        return Ok(offerings);
    }

    [HttpGet("{id}/students")]
    [Authorize(Roles = "Admin,Assistant,Professional,Professor")]
    public async Task<ActionResult<PagedResult<StudentEnrollmentDto>>> GetOfferingStudents(
        Guid id,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null
    )
    {
        var students = await _studentEnrollmentService.GetStudentsByOfferingPaginatedAsync(
            id,
            pageNumber,
            pageSize,
            search
        );
        return Ok(students);
    }

    [HttpGet("students/{studentId}")]
    [Authorize(Roles = "Professor")]
    public async Task<ActionResult<UserDto>> GetStudentDetails(Guid studentId)
    {
        var professorId = GetCurrentUserId();

        var isEnrolled = await _studentEnrollmentService.IsStudentEnrolledWithProfessorAsync(
            studentId,
            professorId
        );
        if (!isEnrolled)
        {
            return Forbid();
        }

        var student = await _userService.GetUserByIdAsync(studentId);
        if (student == null)
        {
            return NotFound();
        }
        return Ok(student);
    }

    [HttpGet("students/{studentId}/observations")]
    [Authorize(Roles = "Professor")]
    public async Task<ActionResult<IEnumerable<PatientObservationDto>>> GetStudentObservations(
        Guid studentId
    )
    {
        var professorId = GetCurrentUserId();

        var isEnrolled = await _studentEnrollmentService.IsStudentEnrolledWithProfessorAsync(
            studentId,
            professorId
        );
        if (!isEnrolled)
        {
            return Forbid();
        }

        var observations = await _patientObservationService.GetByStudentIdAsync(studentId);
        return Ok(observations);
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
