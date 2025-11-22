using IcompCare.Application.DTOs;
using IcompCare.Application.DTOs.Semesters;
using IcompCare.Application.Interfaces;
using IcompCare.Domain.Constants;
using IcompCare.Domain.Entities;
using IcompCare.Domain.Exceptions;
using IcompCare.Domain.Interfaces;

namespace IcompCare.Application.Services;

public class SemesterService : ISemesterService
{
    private readonly ISemesterRepository _semesterRepository;

    public SemesterService(ISemesterRepository semesterRepository)
    {
        _semesterRepository = semesterRepository;
    }

    public async Task<PagedResult<SemesterDto>> GetAllSemestersAsync(
        int pageNumber,
        int pageSize,
        bool includeInactive = false,
        string? search = null
    )
    {
        var (semesters, totalCount) = await _semesterRepository.GetAllAsync(
            pageNumber,
            pageSize,
            includeInactive,
            search
        );

        var semesterDtos = semesters.Select(s => new SemesterDto
        {
            PublicId = s.PublicId,
            Name = s.Name,
            StartDate = s.StartDate,
            EndDate = s.EndDate,
            Status = s.Status,
            CreatedAt = s.CreatedAt,
            UpdatedAt = s.UpdatedAt,
        });

        return new PagedResult<SemesterDto>
        {
            Items = semesterDtos,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
        };
    }

    public async Task<SemesterDto?> GetSemesterByIdAsync(Guid id)
    {
        var semester = await _semesterRepository.GetByPublicIdAsync(id);
        if (semester == null)
            return null;

        return MapToDto(semester);
    }

    public async Task<SemesterDto> CreateSemesterAsync(CreateSemesterDto createSemesterDto)
    {
        if (createSemesterDto.EndDate <= createSemesterDto.StartDate)
        {
            throw new ValidationException(
                "End date must be after start date.",
                DomainErrorCodes.Semester.InvalidDateRange
            );
        }

        var existingSemester = await _semesterRepository.GetByNameAsync(createSemesterDto.Name);
        if (existingSemester != null)
        {
            throw new ConflictException(
                $"Semester with name '{createSemesterDto.Name}' already exists.",
                DomainErrorCodes.Semester.NameAlreadyExists
            );
        }

        var semester = new Semester
        {
            Name = createSemesterDto.Name,
            StartDate = createSemesterDto.StartDate,
            EndDate = createSemesterDto.EndDate,
        };

        var createdSemester = await _semesterRepository.AddAsync(semester);
        return MapToDto(createdSemester);
    }

    public async Task UpdateSemesterAsync(Guid id, UpdateSemesterDto updateSemesterDto)
    {
        var semester = await _semesterRepository.GetByPublicIdAsync(id);
        if (semester == null)
        {
            throw new NotFoundException("Semester", id);
        }

        if (updateSemesterDto.EndDate <= updateSemesterDto.StartDate)
        {
            throw new ValidationException(
                "End date must be after start date.",
                DomainErrorCodes.Semester.InvalidDateRange
            );
        }

        if (semester.Name != updateSemesterDto.Name)
        {
            var existingSemester = await _semesterRepository.GetByNameAsync(updateSemesterDto.Name);
            if (existingSemester != null)
            {
                throw new ConflictException(
                    $"Semester with name '{updateSemesterDto.Name}' already exists.",
                    DomainErrorCodes.Semester.NameAlreadyExists
                );
            }
        }

        semester.Name = updateSemesterDto.Name;
        semester.StartDate = updateSemesterDto.StartDate;
        semester.EndDate = updateSemesterDto.EndDate;
        semester.Status = updateSemesterDto.Status;

        await _semesterRepository.UpdateAsync(semester);
    }

    public async Task DeleteSemesterAsync(Guid id)
    {
        var semester = await _semesterRepository.GetByPublicIdAsync(id);
        if (semester == null)
        {
            throw new NotFoundException("Semester", id);
        }
        await _semesterRepository.DeleteAsync(semester.Id);
    }

    public async Task<SemesterDto?> GetCurrentSemesterAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var semester = await _semesterRepository.GetByDateAsync(today);
        if (semester == null)
            return null;

        return MapToDto(semester);
    }

    private static SemesterDto MapToDto(Semester semester)
    {
        return new SemesterDto
        {
            PublicId = semester.PublicId,
            Name = semester.Name,
            StartDate = semester.StartDate,
            EndDate = semester.EndDate,
            Status = semester.Status,
        };
    }
}
