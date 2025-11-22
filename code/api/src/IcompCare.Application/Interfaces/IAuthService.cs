using IcompCare.Application.DTOs.Auth;

namespace IcompCare.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginDto loginDto);
}
