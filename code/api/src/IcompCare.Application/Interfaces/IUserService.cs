using IcompCare.Application.DTOs;
using IcompCare.Application.DTOs.Users;
using IcompCare.Domain.Enums;

namespace IcompCare.Application.Interfaces;

public interface IUserService
{
    Task<PagedResult<UserDto>> GetAllUsersAsync(
        int pageNumber,
        int pageSize,
        bool includeInactive = false
    );
    Task<PagedResult<UserDto>> GetUsersByRoleAsync(
        UserRole role,
        int pageNumber,
        int pageSize,
        bool includeInactive = false,
        string? search = null
    );
    Task<UserDto?> GetUserByIdAsync(Guid id);
    Task<UserDto> CreateUserAsync(CreateUserDto createUserDto);
    Task UpdateUserAsync(Guid id, UpdateUserDto updateUserDto);
    Task DeleteUserAsync(Guid id);
    Task<IEnumerable<UserDto>> GetActiveProfessionalsAsync();
    Task<IEnumerable<UserDto>> GetActiveUsersByRoleAsync(UserRole role, string? search = null);
    Task ResetPasswordAsync(Guid id, string newPassword);
}
