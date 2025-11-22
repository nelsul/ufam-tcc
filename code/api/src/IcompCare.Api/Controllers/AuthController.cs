using IcompCare.Application.DTOs.Auth;
using IcompCare.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IcompCare.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            var response = await _authService.LoginAsync(loginDto);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                new
                {
                    message = "An error occurred while processing your request.",
                    details = ex.Message,
                }
            );
        }
    }
}
