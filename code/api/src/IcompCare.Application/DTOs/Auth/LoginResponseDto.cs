namespace IcompCare.Application.DTOs.Auth;

using IcompCare.Application.DTOs.Users;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public UserDto User { get; set; } = null!;
}
