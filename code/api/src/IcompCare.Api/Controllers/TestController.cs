using IcompCare.Application.DTOs.Users;
using IcompCare.Application.Interfaces;
using IcompCare.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IcompCare.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly IUserService _userService;

    public TestController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("create-admin")]
    [AllowAnonymous]
    public async Task<ActionResult<UserDto>> CreateAdminUser(CreateUserDto createUserDto)
    {
        createUserDto.Role = UserRole.Admin;

        var createdUser = await _userService.CreateUserAsync(createUserDto);
        return Ok(createdUser);
    }
}
