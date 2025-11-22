using IcompCare.Application.DTOs;
using IcompCare.Application.DTOs.Users;
using IcompCare.Application.Interfaces;
using IcompCare.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IcompCare.Api.Controllers;

[Authorize(Roles = "Admin,Assistant,Professional")]
[ApiController]
[Route("api/[controller]")]
public class ProfessorsController : ControllerBase
{
    private readonly IUserService _userService;

    public ProfessorsController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<UserDto>>> GetProfessors(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool includeInactive = false,
        [FromQuery] string? search = null
    )
    {
        var professors = await _userService.GetUsersByRoleAsync(
            UserRole.Professor,
            pageNumber,
            pageSize,
            includeInactive,
            search
        );
        return Ok(professors);
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateProfessor(CreateUserDto createUserDto)
    {
        createUserDto.Role = UserRole.Professor;
        var createdUser = await _userService.CreateUserAsync(createUserDto);
        return CreatedAtAction(
            nameof(GetProfessor),
            new { id = createdUser.PublicId },
            createdUser
        );
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetProfessor(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null || user.Role != UserRole.Professor)
        {
            return NotFound();
        }
        return Ok(user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProfessor(Guid id, UpdateUserDto updateUserDto)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null || user.Role != UserRole.Professor)
        {
            return NotFound();
        }

        updateUserDto.Role = UserRole.Professor;
        await _userService.UpdateUserAsync(id, updateUserDto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeactivateProfessor(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null || user.Role != UserRole.Professor)
        {
            return NotFound();
        }

        await _userService.DeleteUserAsync(id);
        return NoContent();
    }

    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetActiveProfessors(
        [FromQuery] string? search = null
    )
    {
        var professors = await _userService.GetActiveUsersByRoleAsync(UserRole.Professor, search);
        return Ok(professors);
    }

    [HttpPut("{id}/reset-password")]
    public async Task<IActionResult> ResetPassword(
        Guid id,
        [FromBody] ResetPasswordDto resetPasswordDto
    )
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null || user.Role != UserRole.Professor)
        {
            return NotFound();
        }

        await _userService.ResetPasswordAsync(id, resetPasswordDto.NewPassword);
        return NoContent();
    }
}
