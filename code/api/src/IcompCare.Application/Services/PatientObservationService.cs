using IcompCare.Application.DTOs;
using IcompCare.Application.DTOs.PatientObservations;
using IcompCare.Application.Interfaces;
using IcompCare.Domain.Constants;
using IcompCare.Domain.Entities;
using IcompCare.Domain.Enums;
using IcompCare.Domain.Exceptions;
using IcompCare.Domain.Interfaces;

namespace IcompCare.Application.Services;

public class PatientObservationService : IPatientObservationService
{
    private readonly IPatientObservationRepository _patientObservationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IObservationRepository _observationRepository;

    public PatientObservationService(
        IPatientObservationRepository patientObservationRepository,
        IUserRepository userRepository,
        IObservationRepository observationRepository
    )
    {
        _patientObservationRepository = patientObservationRepository;
        _userRepository = userRepository;
        _observationRepository = observationRepository;
    }

    public async Task<PagedResult<PatientObservationDto>> GetAllAsync(int pageNumber, int pageSize)
    {
        var (items, totalCount) = await _patientObservationRepository.GetAllAsync(
            pageNumber,
            pageSize
        );

        var dtos = items.Select(MapToDto).ToList();

        return new PagedResult<PatientObservationDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
        };
    }

    public async Task<PatientObservationDto> GetByIdAsync(Guid id)
    {
        var patientObservation = await _patientObservationRepository.GetByPublicIdAsync(id);
        if (patientObservation == null)
        {
            throw new NotFoundException(
                "Patient observation not found",
                DomainErrorCodes.PatientObservation.NotFound
            );
        }

        return MapToDto(patientObservation);
    }

    public async Task<IEnumerable<PatientObservationDto>> GetByStudentIdAsync(Guid studentId)
    {
        var student = await _userRepository.GetByPublicIdAsync(studentId);
        if (student == null)
        {
            throw new NotFoundException("Student not found", DomainErrorCodes.User.NotFound);
        }

        var observations = await _patientObservationRepository.GetByStudentIdAsync(student.Id);
        return observations.Select(MapToDto);
    }

    public async Task<PatientObservationDto> CreateAsync(CreatePatientObservationDto dto)
    {
        var student = await _userRepository.GetByPublicIdAsync(dto.StudentId);
        if (student == null)
        {
            throw new NotFoundException("Student not found", DomainErrorCodes.User.NotFound);
        }

        var professional = await _userRepository.GetByPublicIdAsync(dto.ProfessionalId);
        if (professional == null)
        {
            throw new NotFoundException("Professional not found", DomainErrorCodes.User.NotFound);
        }

        var observation = await _observationRepository.GetByPublicIdAsync(dto.ObservationId);
        if (observation == null)
        {
            throw new NotFoundException(
                "Observation type not found",
                DomainErrorCodes.Observation.NotFound
            );
        }

        var patientObservation = new PatientObservation
        {
            StudentId = student.Id,
            ProfessionalId = professional.Id,
            ObservationId = observation.Id,
            Notes = dto.Notes,
            Status = GeneralStatus.Active,
        };

        await _patientObservationRepository.AddAsync(patientObservation);

        patientObservation.Student = student;
        patientObservation.Professional = professional;
        patientObservation.Observation = observation;

        return MapToDto(patientObservation);
    }

    public async Task<PatientObservationDto> UpdateAsync(Guid id, UpdatePatientObservationDto dto)
    {
        var patientObservation = await _patientObservationRepository.GetByPublicIdAsync(id);
        if (patientObservation == null)
        {
            throw new NotFoundException(
                "Patient observation not found",
                DomainErrorCodes.PatientObservation.NotFound
            );
        }

        patientObservation.Notes = dto.Notes;

        await _patientObservationRepository.UpdateAsync(patientObservation);

        return MapToDto(patientObservation);
    }

    public async Task DeleteAsync(Guid id)
    {
        var patientObservation = await _patientObservationRepository.GetByPublicIdAsync(id);
        if (patientObservation == null)
        {
            throw new NotFoundException(
                "Patient observation not found",
                DomainErrorCodes.PatientObservation.NotFound
            );
        }

        await _patientObservationRepository.DeleteAsync(patientObservation.Id);
    }

    private static PatientObservationDto MapToDto(PatientObservation patientObservation)
    {
        return new PatientObservationDto
        {
            Id = patientObservation.PublicId,
            StudentId = patientObservation.Student?.PublicId ?? Guid.Empty,
            StudentName = patientObservation.Student?.Name ?? string.Empty,
            ObservationId = patientObservation.Observation?.PublicId ?? Guid.Empty,
            ObservationName = patientObservation.Observation?.Name ?? string.Empty,
            ProfessionalId = patientObservation.Professional?.PublicId ?? Guid.Empty,
            ProfessionalName = patientObservation.Professional?.Name ?? string.Empty,
            Notes = patientObservation.Notes,
            CreatedAt = patientObservation.CreatedAt,
            UpdatedAt = patientObservation.UpdatedAt,
        };
    }
}
