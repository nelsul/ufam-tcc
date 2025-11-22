using IcompCare.Application.DTOs;
using IcompCare.Application.DTOs.Observations;
using IcompCare.Application.Interfaces;
using IcompCare.Domain.Constants;
using IcompCare.Domain.Entities;
using IcompCare.Domain.Enums;
using IcompCare.Domain.Exceptions;
using IcompCare.Domain.Interfaces;

namespace IcompCare.Application.Services;

public class ObservationService : IObservationService
{
    private readonly IObservationRepository _observationRepository;

    public ObservationService(IObservationRepository observationRepository)
    {
        _observationRepository = observationRepository;
    }

    public async Task<PagedResult<ObservationDto>> GetAllAsync(
        int pageNumber,
        int pageSize,
        bool includeInactive = false,
        string? search = null
    )
    {
        var (items, totalCount) = await _observationRepository.GetAllAsync(
            pageNumber,
            pageSize,
            includeInactive,
            search
        );

        var dtos = items.Select(MapToDto).ToList();

        return new PagedResult<ObservationDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
        };
    }

    public async Task<ObservationDto> GetByIdAsync(Guid id)
    {
        var observation = await _observationRepository.GetByPublicIdAsync(id);
        if (observation == null)
        {
            throw new NotFoundException(
                "Observation not found",
                DomainErrorCodes.Observation.NotFound
            );
        }

        return MapToDto(observation);
    }

    public async Task<ObservationDto> CreateAsync(CreateObservationDto dto)
    {
        if (await _observationRepository.ExistsByNameAsync(dto.Name))
        {
            throw new ConflictException(
                "Observation name already exists",
                DomainErrorCodes.Observation.NameAlreadyExists
            );
        }

        var observation = new Observation
        {
            Name = dto.Name,
            Description = dto.Description,
            Status = GeneralStatus.Active,
        };

        await _observationRepository.AddAsync(observation);

        return MapToDto(observation);
    }

    public async Task<ObservationDto> UpdateAsync(Guid id, UpdateObservationDto dto)
    {
        var observation = await _observationRepository.GetByPublicIdAsync(id);
        if (observation == null)
        {
            throw new NotFoundException(
                "Observation not found",
                DomainErrorCodes.Observation.NotFound
            );
        }

        if (dto.Name != null)
        {
            if (await _observationRepository.ExistsByNameAsync(dto.Name, observation.PublicId))
            {
                throw new ConflictException(
                    "Observation name already exists",
                    DomainErrorCodes.Observation.NameAlreadyExists
                );
            }
            observation.Name = dto.Name;
        }

        if (dto.Description != null)
        {
            observation.Description = dto.Description;
        }

        if (dto.Status != null)
        {
            if (Enum.TryParse<GeneralStatus>(dto.Status, true, out var status))
            {
                observation.Status = status;
            }
        }

        await _observationRepository.UpdateAsync(observation);

        return MapToDto(observation);
    }

    public async Task DeleteAsync(Guid id)
    {
        var observation = await _observationRepository.GetByPublicIdAsync(id);
        if (observation == null)
        {
            throw new NotFoundException(
                "Observation not found",
                DomainErrorCodes.Observation.NotFound
            );
        }

        await _observationRepository.DeleteAsync(observation.Id);
    }

    private static ObservationDto MapToDto(Observation observation)
    {
        return new ObservationDto
        {
            Id = observation.PublicId,
            Name = observation.Name,
            Description = observation.Description,
            Status = observation.Status.ToString(),
            CreatedAt = observation.CreatedAt,
            UpdatedAt = observation.UpdatedAt,
        };
    }
}
