using IcompCare.Application.DTOs;
using IcompCare.Application.DTOs.SessionTypes;
using IcompCare.Application.Interfaces;
using IcompCare.Domain.Constants;
using IcompCare.Domain.Entities;
using IcompCare.Domain.Exceptions;
using IcompCare.Domain.Interfaces;

namespace IcompCare.Application.Services;

public class SessionTypeService : ISessionTypeService
{
    private readonly ISessionTypeRepository _sessionTypeRepository;

    public SessionTypeService(ISessionTypeRepository sessionTypeRepository)
    {
        _sessionTypeRepository = sessionTypeRepository;
    }

    public async Task<PagedResult<SessionTypeDto>> GetAllSessionTypesAsync(
        int pageNumber,
        int pageSize,
        bool includeInactive = false,
        string? search = null
    )
    {
        var (sessionTypes, totalCount) = await _sessionTypeRepository.GetAllAsync(
            pageNumber,
            pageSize,
            includeInactive,
            search
        );

        var sessionTypeDtos = sessionTypes.Select(MapToDto);

        return new PagedResult<SessionTypeDto>
        {
            Items = sessionTypeDtos,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
        };
    }

    public async Task<SessionTypeDto?> GetSessionTypeByIdAsync(Guid id)
    {
        var sessionType = await _sessionTypeRepository.GetByPublicIdAsync(id);
        if (sessionType == null)
            return null;

        return MapToDto(sessionType);
    }

    public async Task<SessionTypeDto> CreateSessionTypeAsync(
        CreateSessionTypeDto createSessionTypeDto
    )
    {
        if (createSessionTypeDto.DurationMinutes <= 0)
        {
            throw new ValidationException(
                "Duration must be greater than 0.",
                DomainErrorCodes.SessionType.InvalidDuration
            );
        }

        var existingSessionType = await _sessionTypeRepository.GetByNameAsync(
            createSessionTypeDto.Name
        );
        if (existingSessionType != null)
        {
            throw new ConflictException(
                $"Session type with name '{createSessionTypeDto.Name}' already exists.",
                DomainErrorCodes.SessionType.NameAlreadyExists
            );
        }

        var sessionType = new SessionType
        {
            Name = createSessionTypeDto.Name,
            DurationMinutes = createSessionTypeDto.DurationMinutes,
            Description = createSessionTypeDto.Description,
        };

        var createdSessionType = await _sessionTypeRepository.AddAsync(sessionType);
        return MapToDto(createdSessionType);
    }

    public async Task UpdateSessionTypeAsync(Guid id, UpdateSessionTypeDto updateSessionTypeDto)
    {
        if (updateSessionTypeDto.DurationMinutes <= 0)
        {
            throw new ValidationException(
                "Duration must be greater than 0.",
                DomainErrorCodes.SessionType.InvalidDuration
            );
        }

        var sessionType = await _sessionTypeRepository.GetByPublicIdAsync(id);
        if (sessionType == null)
        {
            throw new NotFoundException("SessionType", id);
        }

        if (sessionType.Name != updateSessionTypeDto.Name)
        {
            var existingSessionType = await _sessionTypeRepository.GetByNameAsync(
                updateSessionTypeDto.Name
            );
            if (existingSessionType != null)
            {
                throw new ConflictException(
                    $"Session type with name '{updateSessionTypeDto.Name}' already exists.",
                    DomainErrorCodes.SessionType.NameAlreadyExists
                );
            }
        }

        sessionType.Name = updateSessionTypeDto.Name;
        sessionType.DurationMinutes = updateSessionTypeDto.DurationMinutes;
        sessionType.Description = updateSessionTypeDto.Description;
        sessionType.Status = updateSessionTypeDto.Status;

        await _sessionTypeRepository.UpdateAsync(sessionType);
    }

    public async Task DeleteSessionTypeAsync(Guid id)
    {
        var sessionType = await _sessionTypeRepository.GetByPublicIdAsync(id);
        if (sessionType == null)
        {
            throw new NotFoundException("SessionType", id);
        }

        await _sessionTypeRepository.DeleteAsync(sessionType.Id);
    }

    private static SessionTypeDto MapToDto(SessionType sessionType)
    {
        return new SessionTypeDto
        {
            PublicId = sessionType.PublicId,
            Name = sessionType.Name,
            DurationMinutes = sessionType.DurationMinutes,
            Description = sessionType.Description,
            Status = sessionType.Status,
            CreatedAt = sessionType.CreatedAt,
            UpdatedAt = sessionType.UpdatedAt,
        };
    }
}
