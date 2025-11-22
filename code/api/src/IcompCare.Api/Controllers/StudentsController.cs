using IcompCare.Application.DTOs;
using IcompCare.Application.DTOs.PatientObservations;
using IcompCare.Application.DTOs.PatientRecords;
using IcompCare.Application.DTOs.StudentEnrollments;
using IcompCare.Application.DTOs.Users;
using IcompCare.Application.Interfaces;
using IcompCare.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IcompCare.Api.Controllers;

[Authorize(Roles = "Admin,Assistant,Professional")]
[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IPatientRecordService _patientRecordService;
    private readonly IPatientObservationService _patientObservationService;
    private readonly IStudentEnrollmentService _studentEnrollmentService;

    public StudentsController(
        IUserService userService,
        IPatientRecordService patientRecordService,
        IPatientObservationService patientObservationService,
        IStudentEnrollmentService studentEnrollmentService
    )
    {
        _userService = userService;
        _patientRecordService = patientRecordService;
        _patientObservationService = patientObservationService;
        _studentEnrollmentService = studentEnrollmentService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<UserDto>>> GetStudents(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool includeInactive = false,
        [FromQuery] string? search = null
    )
    {
        var students = await _userService.GetUsersByRoleAsync(
            UserRole.Student,
            pageNumber,
            pageSize,
            includeInactive,
            search
        );
        return Ok(students);
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateStudent(CreateUserDto createUserDto)
    {
        createUserDto.Role = UserRole.Student;
        var createdUser = await _userService.CreateUserAsync(createUserDto);
        return CreatedAtAction(nameof(GetStudent), new { id = createdUser.PublicId }, createdUser);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetStudent(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null || user.Role != UserRole.Student)
        {
            return NotFound();
        }
        return Ok(user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStudent(Guid id, UpdateUserDto updateUserDto)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null || user.Role != UserRole.Student)
        {
            return NotFound();
        }

        updateUserDto.Role = UserRole.Student;
        await _userService.UpdateUserAsync(id, updateUserDto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeactivateStudent(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null || user.Role != UserRole.Student)
        {
            return NotFound();
        }

        await _userService.DeleteUserAsync(id);
        return NoContent();
    }

    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetActiveStudents(
        [FromQuery] string? search = null
    )
    {
        var students = await _userService.GetActiveUsersByRoleAsync(UserRole.Student, search);
        return Ok(students);
    }

    [HttpGet("{id}/patient-record")]
    public async Task<ActionResult<PatientRecordDto>> GetStudentPatientRecord(Guid id)
    {
        var records = await _patientRecordService.GetByStudentIdAsync(id);
        var record = records.FirstOrDefault();

        if (record == null)
        {
            return NotFound();
        }

        return Ok(record);
    }

    [HttpGet("{id}/patient-observations")]
    public async Task<
        ActionResult<IEnumerable<PatientObservationDto>>
    > GetStudentPatientObservations(Guid id)
    {
        var observations = await _patientObservationService.GetByStudentIdAsync(id);
        return Ok(observations);
    }

    [HttpGet("{id}/enrollments")]
    public async Task<ActionResult<IEnumerable<StudentEnrollmentDto>>> GetStudentEnrollments(
        Guid id
    )
    {
        var enrollments = await _studentEnrollmentService.GetByStudentIdAsync(id);
        return Ok(enrollments);
    }

    [HttpPost("{id}/enrollments")]
    public async Task<ActionResult<StudentEnrollmentDto>> EnrollStudent(
        Guid id,
        CreateStudentEnrollmentDto dto
    )
    {
        dto.StudentId = id;
        var enrollment = await _studentEnrollmentService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetStudentEnrollments), new { id = id }, enrollment);
    }

    [HttpDelete("{id}/enrollments/{enrollmentId}")]
    public async Task<IActionResult> RemoveEnrollment(Guid id, Guid enrollmentId)
    {
        await _studentEnrollmentService.DeleteAsync(enrollmentId);
        return NoContent();
    }

    [HttpPut("{id}/reset-password")]
    public async Task<IActionResult> ResetPassword(
        Guid id,
        [FromBody] ResetPasswordDto resetPasswordDto
    )
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null || user.Role != UserRole.Student)
        {
            return NotFound();
        }

        await _userService.ResetPasswordAsync(id, resetPasswordDto.NewPassword);
        return NoContent();
    }
}
