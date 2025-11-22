using IcompCare.Application.DTOs;
using IcompCare.Application.DTOs.Semesters;
using IcompCare.Application.DTOs.SubjectOfferings;
using IcompCare.Application.DTOs.Subjects;
using IcompCare.Application.DTOs.Users;
using IcompCare.Application.Interfaces;
using IcompCare.Domain.Constants;
using IcompCare.Domain.Entities;
using IcompCare.Domain.Exceptions;
using IcompCare.Domain.Interfaces;

namespace IcompCare.Application.Services;

public class SubjectOfferingService : ISubjectOfferingService
{
    private readonly ISubjectOfferingRepository _subjectOfferingRepository;
    private readonly ISemesterRepository _semesterRepository;
    private readonly ISubjectRepository _subjectRepository;
    private readonly IUserRepository _userRepository;

    public SubjectOfferingService(
        ISubjectOfferingRepository subjectOfferingRepository,
        ISemesterRepository semesterRepository,
        ISubjectRepository subjectRepository,
        IUserRepository userRepository
    )
    {
        _subjectOfferingRepository = subjectOfferingRepository;
        _semesterRepository = semesterRepository;
        _subjectRepository = subjectRepository;
        _userRepository = userRepository;
    }

    public async Task<PagedResult<SubjectOfferingDto>> GetAllSubjectOfferingsAsync(
        int pageNumber,
        int pageSize,
        bool includeInactive = false,
        string? search = null
    )
    {
        var (subjectOfferings, totalCount) = await _subjectOfferingRepository.GetAllAsync(
            pageNumber,
            pageSize,
            includeInactive,
            search
        );

        var subjectOfferingDtos = subjectOfferings.Select(MapToDto);

        return new PagedResult<SubjectOfferingDto>
        {
            Items = subjectOfferingDtos,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
        };
    }

    public async Task<SubjectOfferingDto?> GetSubjectOfferingByIdAsync(Guid id)
    {
        var subjectOffering = await _subjectOfferingRepository.GetByPublicIdAsync(id);
        if (subjectOffering == null)
            return null;

        return MapToDto(subjectOffering);
    }

    public async Task<SubjectOfferingDto> CreateSubjectOfferingAsync(
        CreateSubjectOfferingDto createSubjectOfferingDto
    )
    {
        var semester = await _semesterRepository.GetByPublicIdAsync(
            createSubjectOfferingDto.SemesterId
        );
        if (semester == null)
        {
            throw new NotFoundException("Semester", createSubjectOfferingDto.SemesterId);
        }

        var subject = await _subjectRepository.GetByPublicIdAsync(
            createSubjectOfferingDto.SubjectId
        );
        if (subject == null)
        {
            throw new NotFoundException("Subject", createSubjectOfferingDto.SubjectId);
        }

        var professor = await _userRepository.GetByPublicIdAsync(
            createSubjectOfferingDto.ProfessorId
        );
        if (professor == null)
        {
            throw new NotFoundException("Professor", createSubjectOfferingDto.ProfessorId);
        }

        var existingOffering = await _subjectOfferingRepository.GetByUniqueKeyAsync(
            semester.Id,
            subject.Id,
            professor.Id
        );
        if (existingOffering != null)
        {
            throw new ConflictException(
                "This subject offering already exists.",
                DomainErrorCodes.SubjectOffering.AlreadyExists
            );
        }

        var subjectOffering = new SubjectOffering
        {
            SemesterId = semester.Id,
            SubjectId = subject.Id,
            ProfessorId = professor.Id,
        };

        var createdOffering = await _subjectOfferingRepository.AddAsync(subjectOffering);

        createdOffering.Semester = semester;
        createdOffering.Subject = subject;
        createdOffering.Professor = professor;

        return MapToDto(createdOffering);
    }

    public async Task UpdateSubjectOfferingAsync(
        Guid id,
        UpdateSubjectOfferingDto updateSubjectOfferingDto
    )
    {
        var subjectOffering = await _subjectOfferingRepository.GetByPublicIdAsync(id);
        if (subjectOffering == null)
        {
            throw new NotFoundException("SubjectOffering", id);
        }

        var semester = await _semesterRepository.GetByPublicIdAsync(
            updateSubjectOfferingDto.SemesterId
        );
        if (semester == null)
        {
            throw new NotFoundException("Semester", updateSubjectOfferingDto.SemesterId);
        }

        var subject = await _subjectRepository.GetByPublicIdAsync(
            updateSubjectOfferingDto.SubjectId
        );
        if (subject == null)
        {
            throw new NotFoundException("Subject", updateSubjectOfferingDto.SubjectId);
        }

        var professor = await _userRepository.GetByPublicIdAsync(
            updateSubjectOfferingDto.ProfessorId
        );
        if (professor == null)
        {
            throw new NotFoundException("Professor", updateSubjectOfferingDto.ProfessorId);
        }

        if (
            subjectOffering.SemesterId != semester.Id
            || subjectOffering.SubjectId != subject.Id
            || subjectOffering.ProfessorId != professor.Id
        )
        {
            var existingOffering = await _subjectOfferingRepository.GetByUniqueKeyAsync(
                semester.Id,
                subject.Id,
                professor.Id
            );
            if (existingOffering != null && existingOffering.PublicId != id)
            {
                throw new ConflictException(
                    "This subject offering already exists.",
                    DomainErrorCodes.SubjectOffering.AlreadyExists
                );
            }
        }

        subjectOffering.SemesterId = semester.Id;
        subjectOffering.SubjectId = subject.Id;
        subjectOffering.ProfessorId = professor.Id;
        subjectOffering.Status = updateSubjectOfferingDto.Status;

        await _subjectOfferingRepository.UpdateAsync(subjectOffering);
    }

    public async Task DeleteSubjectOfferingAsync(Guid id)
    {
        var subjectOffering = await _subjectOfferingRepository.GetByPublicIdAsync(id);
        if (subjectOffering == null)
        {
            throw new NotFoundException("SubjectOffering", id);
        }
        await _subjectOfferingRepository.DeleteAsync(subjectOffering.Id);
    }

    public async Task<PagedResult<SubjectOfferingDto>> GetActiveOfferingsByDateAsync(
        DateOnly date,
        int pageNumber,
        int pageSize
    )
    {
        var (offerings, totalCount) =
            await _subjectOfferingRepository.GetActiveOfferingsByDateAsync(
                date,
                pageNumber,
                pageSize
            );

        var dtos = offerings.Select(so => new SubjectOfferingDto
        {
            PublicId = so.PublicId,
            Status = so.Status,
            CreatedAt = so.CreatedAt,
            UpdatedAt = so.UpdatedAt,
            Semester = new SemesterDto
            {
                PublicId = so.Semester.PublicId,
                Name = so.Semester.Name,
                StartDate = so.Semester.StartDate,
                EndDate = so.Semester.EndDate,
                Status = so.Semester.Status,
                CreatedAt = so.Semester.CreatedAt,
                UpdatedAt = so.Semester.UpdatedAt,
            },
            Subject = new SubjectDto
            {
                PublicId = so.Subject.PublicId,
                Name = so.Subject.Name,
                Code = so.Subject.Code,
                Description = so.Subject.Description,
                Status = so.Subject.Status,
                CreatedAt = so.Subject.CreatedAt,
                UpdatedAt = so.Subject.UpdatedAt,
            },
            Professor = new UserDto
            {
                PublicId = so.Professor.PublicId,
                Name = so.Professor.Name,
                FullName = so.Professor.FullName,
                InstitutionalEmail = so.Professor.InstitutionalEmail,
                Registration = so.Professor.Registration,
                Status = so.Professor.Status,
                Role = so.Professor.Role,
                CreatedAt = so.Professor.CreatedAt,
                UpdatedAt = so.Professor.UpdatedAt,
            },
        });

        return new PagedResult<SubjectOfferingDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
        };
    }

    public async Task<PagedResult<SubjectOfferingDto>> GetByProfessorIdAsync(
        Guid professorPublicId,
        int pageNumber,
        int pageSize,
        bool includeInactive = false,
        string? search = null
    )
    {
        var (offerings, totalCount) = await _subjectOfferingRepository.GetByProfessorIdAsync(
            professorPublicId,
            pageNumber,
            pageSize,
            includeInactive,
            search
        );

        var subjectOfferingDtos = offerings.Select(MapToDto);

        return new PagedResult<SubjectOfferingDto>
        {
            Items = subjectOfferingDtos,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
        };
    }

    private static SubjectOfferingDto MapToDto(SubjectOffering subjectOffering)
    {
        return new SubjectOfferingDto
        {
            PublicId = subjectOffering.PublicId,
            Semester = new SemesterDto
            {
                PublicId = subjectOffering.Semester.PublicId,
                Name = subjectOffering.Semester.Name,
                StartDate = subjectOffering.Semester.StartDate,
                EndDate = subjectOffering.Semester.EndDate,
                Status = subjectOffering.Semester.Status,
            },
            Subject = new SubjectDto
            {
                PublicId = subjectOffering.Subject.PublicId,
                Name = subjectOffering.Subject.Name,
                Code = subjectOffering.Subject.Code,
                Description = subjectOffering.Subject.Description,
                Status = subjectOffering.Subject.Status,
            },
            Professor = new UserDto
            {
                PublicId = subjectOffering.Professor.PublicId,
                Name = subjectOffering.Professor.Name,
                FullName = subjectOffering.Professor.FullName,
                InstitutionalEmail = subjectOffering.Professor.InstitutionalEmail,
                Registration = subjectOffering.Professor.Registration,
                Status = subjectOffering.Professor.Status,
                Role = subjectOffering.Professor.Role,
            },
            Status = subjectOffering.Status,
            CreatedAt = subjectOffering.CreatedAt,
            UpdatedAt = subjectOffering.UpdatedAt,
        };
    }
}
