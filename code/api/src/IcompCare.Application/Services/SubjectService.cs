using IcompCare.Application.DTOs;
using IcompCare.Application.DTOs.Subjects;
using IcompCare.Application.Interfaces;
using IcompCare.Domain.Constants;
using IcompCare.Domain.Entities;
using IcompCare.Domain.Exceptions;
using IcompCare.Domain.Interfaces;

namespace IcompCare.Application.Services;

public class SubjectService : ISubjectService
{
    private readonly ISubjectRepository _subjectRepository;

    public SubjectService(ISubjectRepository subjectRepository)
    {
        _subjectRepository = subjectRepository;
    }

    public async Task<PagedResult<SubjectDto>> GetAllSubjectsAsync(
        int pageNumber,
        int pageSize,
        bool includeInactive = false,
        string? search = null
    )
    {
        var (subjects, totalCount) = await _subjectRepository.GetAllAsync(
            pageNumber,
            pageSize,
            includeInactive,
            search
        );

        var subjectDtos = subjects.Select(s => new SubjectDto
        {
            PublicId = s.PublicId,
            Name = s.Name,
            Code = s.Code,
            Description = s.Description,
            Status = s.Status,
            CreatedAt = s.CreatedAt,
            UpdatedAt = s.UpdatedAt,
        });

        return new PagedResult<SubjectDto>
        {
            Items = subjectDtos,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
        };
    }

    public async Task<SubjectDto?> GetSubjectByIdAsync(Guid id)
    {
        var subject = await _subjectRepository.GetByPublicIdAsync(id);
        if (subject == null)
            return null;

        return MapToDto(subject);
    }

    public async Task<SubjectDto> CreateSubjectAsync(CreateSubjectDto createSubjectDto)
    {
        var existingSubject = await _subjectRepository.GetByCodeAsync(createSubjectDto.Code);
        if (existingSubject != null)
        {
            throw new ConflictException(
                $"Subject with code '{createSubjectDto.Code}' already exists.",
                DomainErrorCodes.Subject.CodeAlreadyExists
            );
        }

        var subject = new Subject
        {
            Name = createSubjectDto.Name,
            Code = createSubjectDto.Code,
            Description = createSubjectDto.Description,
        };

        var createdSubject = await _subjectRepository.AddAsync(subject);
        return MapToDto(createdSubject);
    }

    public async Task UpdateSubjectAsync(Guid id, UpdateSubjectDto updateSubjectDto)
    {
        var subject = await _subjectRepository.GetByPublicIdAsync(id);
        if (subject == null)
        {
            throw new NotFoundException("Subject", id);
        }

        if (subject.Code != updateSubjectDto.Code)
        {
            var existingSubject = await _subjectRepository.GetByCodeAsync(updateSubjectDto.Code);
            if (existingSubject != null)
            {
                throw new ConflictException(
                    $"Subject with code '{updateSubjectDto.Code}' already exists.",
                    DomainErrorCodes.Subject.CodeAlreadyExists
                );
            }
        }

        subject.Name = updateSubjectDto.Name;
        subject.Code = updateSubjectDto.Code;
        subject.Description = updateSubjectDto.Description;
        subject.Status = updateSubjectDto.Status;

        await _subjectRepository.UpdateAsync(subject);
    }

    public async Task DeleteSubjectAsync(Guid id)
    {
        var subject = await _subjectRepository.GetByPublicIdAsync(id);
        if (subject == null)
        {
            throw new NotFoundException("Subject", id);
        }
        await _subjectRepository.DeleteAsync(subject.Id);
    }

    public async Task<IEnumerable<SubjectDto>> GetActiveSubjectsAsync(string? search = null)
    {
        var subjects = await _subjectRepository.GetActiveSubjectsAsync(search);
        return subjects.Select(MapToDto);
    }

    private static SubjectDto MapToDto(Subject subject)
    {
        return new SubjectDto
        {
            PublicId = subject.PublicId,
            Name = subject.Name,
            Code = subject.Code,
            Description = subject.Description,
            Status = subject.Status,
        };
    }
}
