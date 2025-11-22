using System.Text.RegularExpressions;
using IcompCare.Application.DTOs;
using IcompCare.Application.DTOs.Users;
using IcompCare.Application.Interfaces;
using IcompCare.Domain.Constants;
using IcompCare.Domain.Entities;
using IcompCare.Domain.Enums;
using IcompCare.Domain.Exceptions;
using IcompCare.Domain.Interfaces;

namespace IcompCare.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private const string EmailRegexPattern = @"^[A-Za-z0-9._+%-]+@[A-Za-z0-9.-]+\.[A-Za-z]+$";

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<PagedResult<UserDto>> GetAllUsersAsync(
        int pageNumber,
        int pageSize,
        bool includeInactive = false
    )
    {
        var (users, totalCount) = await _userRepository.GetAllAsync(
            pageNumber,
            pageSize,
            includeInactive
        );

        var userDtos = users.Select(u => new UserDto
        {
            PublicId = u.PublicId,
            Name = u.Name,
            FullName = u.FullName,
            InstitutionalEmail = u.InstitutionalEmail,
            Registration = u.Registration,
            Status = u.Status,
            Role = u.Role,
            CreatedAt = u.CreatedAt,
            UpdatedAt = u.UpdatedAt,
        });

        return new PagedResult<UserDto>
        {
            Items = userDtos,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
        };
    }

    public async Task<PagedResult<UserDto>> GetUsersByRoleAsync(
        UserRole role,
        int pageNumber,
        int pageSize,
        bool includeInactive = false,
        string? search = null
    )
    {
        var (users, totalCount) = await _userRepository.GetByRoleAsync(
            role,
            pageNumber,
            pageSize,
            includeInactive,
            search
        );

        var userDtos = users.Select(u => new UserDto
        {
            PublicId = u.PublicId,
            Name = u.Name,
            FullName = u.FullName,
            InstitutionalEmail = u.InstitutionalEmail,
            Registration = u.Registration,
            Status = u.Status,
            Role = u.Role,
            CreatedAt = u.CreatedAt,
            UpdatedAt = u.UpdatedAt,
        });

        return new PagedResult<UserDto>
        {
            Items = userDtos,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
        };
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByPublicIdAsync(id);
        if (user == null)
            return null;

        return MapToDto(user);
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
    {
        await ValidateUserAsync(createUserDto.InstitutionalEmail, createUserDto.Registration);

        var user = new User
        {
            Name = createUserDto.Name,
            FullName = createUserDto.FullName,
            InstitutionalEmail = createUserDto.InstitutionalEmail,
            Registration = createUserDto.Registration,
            Role = createUserDto.Role,
            PasswordHash = !string.IsNullOrEmpty(createUserDto.Password)
                ? BCrypt.Net.BCrypt.HashPassword(createUserDto.Password)
                : null,
        };

        var createdUser = await _userRepository.AddAsync(user);
        return MapToDto(createdUser);
    }

    public async Task UpdateUserAsync(Guid id, UpdateUserDto updateUserDto)
    {
        var user = await _userRepository.GetByPublicIdAsync(id);
        if (user == null)
        {
            throw new NotFoundException("User", id);
        }

        await ValidateUserAsync(
            updateUserDto.InstitutionalEmail,
            updateUserDto.Registration,
            user.PublicId
        );

        user.Name = updateUserDto.Name;
        user.FullName = updateUserDto.FullName;
        user.InstitutionalEmail = updateUserDto.InstitutionalEmail;
        user.Registration = updateUserDto.Registration;
        user.Status = updateUserDto.Status;
        user.Role = updateUserDto.Role;

        await _userRepository.UpdateAsync(user);
    }

    public async Task DeleteUserAsync(Guid id)
    {
        var user = await _userRepository.GetByPublicIdAsync(id);
        if (user == null)
        {
            throw new NotFoundException("User", id);
        }
        await _userRepository.DeleteAsync(user.Id);
    }

    public async Task<IEnumerable<UserDto>> GetActiveProfessionalsAsync()
    {
        var users = await _userRepository.GetAllAsync(false);
        var professionals = users
            .Where(u => u.Role == UserRole.Professional && u.Status == UserStatus.Active)
            .Select(MapToDto);

        return professionals;
    }

    public async Task<IEnumerable<UserDto>> GetActiveUsersByRoleAsync(
        UserRole role,
        string? search = null
    )
    {
        var users = await _userRepository.GetActiveUsersByRoleAsync(role, search);
        return users.Select(MapToDto);
    }

    public async Task ResetPasswordAsync(Guid id, string newPassword)
    {
        var user = await _userRepository.GetByPublicIdAsync(id);
        if (user == null)
        {
            throw new NotFoundException("User", id);
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        await _userRepository.UpdateAsync(user);
    }

    private async Task ValidateUserAsync(
        string email,
        string registration,
        Guid? currentUserId = null
    )
    {
        if (!Regex.IsMatch(email, EmailRegexPattern))
        {
            throw new ValidationException(
                "Invalid email format.",
                DomainErrorCodes.User.InvalidEmail
            );
        }

        var existingUserByEmail = await _userRepository.GetByEmailAsync(email);
        if (
            existingUserByEmail != null
            && (currentUserId == null || existingUserByEmail.PublicId != currentUserId)
        )
        {
            throw new ConflictException(
                $"A user with email '{email}' already exists.",
                DomainErrorCodes.User.EmailAlreadyRegistered
            );
        }

        var existingUserByRegistration = await _userRepository.GetByRegistrationAsync(registration);
        if (
            existingUserByRegistration != null
            && (currentUserId == null || existingUserByRegistration.PublicId != currentUserId)
        )
        {
            throw new ConflictException(
                $"A user with registration '{registration}' already exists.",
                DomainErrorCodes.User.RegistrationAlreadyExists
            );
        }
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            PublicId = user.PublicId,
            Name = user.Name,
            FullName = user.FullName,
            InstitutionalEmail = user.InstitutionalEmail,
            Registration = user.Registration,
            Status = user.Status,
            Role = user.Role,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
        };
    }
}
