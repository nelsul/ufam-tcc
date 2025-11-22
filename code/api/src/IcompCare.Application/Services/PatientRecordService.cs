using IcompCare.Application.DTOs;
using IcompCare.Application.DTOs.PatientRecords;
using IcompCare.Application.Interfaces;
using IcompCare.Domain.Constants;
using IcompCare.Domain.Entities;
using IcompCare.Domain.Enums;
using IcompCare.Domain.Exceptions;
using IcompCare.Domain.Interfaces;

namespace IcompCare.Application.Services;

public class PatientRecordService : IPatientRecordService
{
    private readonly IPatientRecordRepository _patientRecordRepository;
    private readonly IUserRepository _userRepository;

    public PatientRecordService(
        IPatientRecordRepository patientRecordRepository,
        IUserRepository userRepository
    )
    {
        _patientRecordRepository = patientRecordRepository;
        _userRepository = userRepository;
    }

    public async Task<PagedResult<PatientRecordDto>> GetAllAsync(int pageNumber, int pageSize)
    {
        var (items, totalCount) = await _patientRecordRepository.GetAllAsync(pageNumber, pageSize);

        var dtos = items.Select(MapToDto).ToList();

        return new PagedResult<PatientRecordDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
        };
    }

    public async Task<PatientRecordDto> GetByIdAsync(Guid id)
    {
        var patientRecord = await _patientRecordRepository.GetByPublicIdAsync(id);
        if (patientRecord == null)
        {
            throw new NotFoundException(
                "Patient record not found",
                DomainErrorCodes.PatientRecord.NotFound
            );
        }

        return MapToDto(patientRecord);
    }

    public async Task<IEnumerable<PatientRecordDto>> GetByStudentIdAsync(Guid studentId)
    {
        var student = await _userRepository.GetByPublicIdAsync(studentId);
        if (student == null)
        {
            throw new NotFoundException("Student not found", DomainErrorCodes.User.NotFound);
        }

        var records = await _patientRecordRepository.GetByStudentIdAsync(student.Id);
        return records.Select(MapToDto);
    }

    public async Task<PatientRecordDto> CreateAsync(CreatePatientRecordDto dto)
    {
        var student = await _userRepository.GetByPublicIdAsync(dto.StudentId);
        if (student == null)
        {
            throw new NotFoundException("Student not found", DomainErrorCodes.User.NotFound);
        }

        var patientRecord = new PatientRecord
        {
            StudentId = student.Id,
            Content = dto.Content,
            Status = GeneralStatus.Active,
        };

        await _patientRecordRepository.AddAsync(patientRecord);

        patientRecord.Student = student;

        return MapToDto(patientRecord);
    }

    public async Task<PatientRecordDto> UpdateAsync(Guid id, UpdatePatientRecordDto dto)
    {
        var patientRecord = await _patientRecordRepository.GetByPublicIdAsync(id);
        if (patientRecord == null)
        {
            throw new NotFoundException(
                "Patient record not found",
                DomainErrorCodes.PatientRecord.NotFound
            );
        }

        patientRecord.Content = dto.Content;

        await _patientRecordRepository.UpdateAsync(patientRecord);

        return MapToDto(patientRecord);
    }

    public async Task DeleteAsync(Guid id)
    {
        var patientRecord = await _patientRecordRepository.GetByPublicIdAsync(id);
        if (patientRecord == null)
        {
            throw new NotFoundException(
                "Patient record not found",
                DomainErrorCodes.PatientRecord.NotFound
            );
        }

        await _patientRecordRepository.DeleteAsync(patientRecord.Id);
    }

    private static PatientRecordDto MapToDto(PatientRecord patientRecord)
    {
        return new PatientRecordDto
        {
            Id = patientRecord.PublicId,
            StudentId = patientRecord.Student?.PublicId ?? Guid.Empty,
            StudentName = patientRecord.Student?.Name ?? string.Empty,
            Content = patientRecord.Content,
            CreatedAt = patientRecord.CreatedAt,
            UpdatedAt = patientRecord.UpdatedAt,
        };
    }
}
