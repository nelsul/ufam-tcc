using IcompCare.Application.DTOs;
using IcompCare.Application.DTOs.Sessions;
using IcompCare.Application.Interfaces;
using IcompCare.Domain.Constants;
using IcompCare.Domain.Entities;
using IcompCare.Domain.Enums;
using IcompCare.Domain.Exceptions;
using IcompCare.Domain.Interfaces;

namespace IcompCare.Application.Services;

public class SessionService : ISessionService
{
    private readonly ISessionRepository _sessionRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IUserRepository _userRepository;

    public SessionService(
        ISessionRepository sessionRepository,
        IAppointmentRepository appointmentRepository,
        IUserRepository userRepository
    )
    {
        _sessionRepository = sessionRepository;
        _appointmentRepository = appointmentRepository;
        _userRepository = userRepository;
    }

    public async Task<PagedResult<SessionDto>> GetAllAsync(int pageNumber, int pageSize)
    {
        var (items, totalCount) = await _sessionRepository.GetAllAsync(pageNumber, pageSize);

        var dtos = items.Select(MapToDto).ToList();

        return new PagedResult<SessionDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
        };
    }

    public async Task<PagedResult<SessionDto>> GetByProfessionalAsync(
        Guid professionalId,
        int pageNumber,
        int pageSize,
        DateTimeOffset? dateFrom = null,
        DateTimeOffset? dateTo = null,
        string? search = null
    )
    {
        var professional = await _userRepository.GetByPublicIdAsync(professionalId);
        if (professional == null)
        {
            throw new NotFoundException("Professional not found", DomainErrorCodes.User.NotFound);
        }

        var (items, totalCount) = await _sessionRepository.GetByProfessionalIdAsync(
            professional.Id,
            pageNumber,
            pageSize,
            dateFrom,
            dateTo,
            search
        );

        var dtos = items.Select(MapToDto).ToList();

        return new PagedResult<SessionDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
        };
    }

    public async Task<SessionDto?> GetOpenSessionByProfessionalAsync(Guid professionalId)
    {
        var professional = await _userRepository.GetByPublicIdAsync(professionalId);
        if (professional == null)
        {
            throw new NotFoundException("Professional not found", DomainErrorCodes.User.NotFound);
        }

        var session = await _sessionRepository.GetOpenSessionByProfessionalIdAsync(professional.Id);
        return session != null ? MapToDto(session) : null;
    }

    public async Task<SessionDto> GetByIdAsync(Guid id)
    {
        var session = await _sessionRepository.GetByPublicIdAsync(id);
        if (session == null)
        {
            throw new NotFoundException("Session not found", DomainErrorCodes.Session.NotFound);
        }

        return MapToDto(session);
    }

    public async Task<SessionDto> CreateAsync(CreateSessionDto dto)
    {
        var appointment = await _appointmentRepository.GetByPublicIdAsync(dto.AppointmentId);
        if (appointment == null)
        {
            throw new NotFoundException(
                "Appointment not found",
                DomainErrorCodes.Appointment.NotFound
            );
        }

        var existingSession = await _sessionRepository.GetByAppointmentIdAsync(appointment.Id);
        if (existingSession != null)
        {
            throw new ConflictException(
                "Session already exists for this appointment",
                DomainErrorCodes.Session.AlreadyExistsForAppointment
            );
        }

        var professional = await _userRepository.GetByPublicIdAsync(dto.ProfessionalId);
        if (professional == null)
        {
            throw new NotFoundException("Professional not found", DomainErrorCodes.User.NotFound);
        }

        var student = await _userRepository.GetByPublicIdAsync(dto.StudentId);
        if (student == null)
        {
            throw new NotFoundException("Student not found", DomainErrorCodes.User.NotFound);
        }

        var session = new Session
        {
            AppointmentId = appointment.Id,
            ProfessionalId = professional.Id,
            StudentId = student.Id,
            StartedAt = dto.StartedAt,
            Notes = dto.Notes,
            Status = SessionStatus.InProgress,
        };

        await _sessionRepository.AddAsync(session);

        session.Appointment = appointment;
        session.Professional = professional;
        session.Student = student;

        return MapToDto(session);
    }

    public async Task<SessionDto> StartSessionAsync(Guid appointmentId, Guid professionalId)
    {
        var professional = await _userRepository.GetByPublicIdAsync(professionalId);
        if (professional == null)
        {
            throw new NotFoundException("Professional not found", DomainErrorCodes.User.NotFound);
        }

        var openSession = await _sessionRepository.GetOpenSessionByProfessionalIdAsync(
            professional.Id
        );
        if (openSession != null)
        {
            throw new ConflictException(
                "Professional already has an open session",
                DomainErrorCodes.Session.ProfessionalHasOpenSession
            );
        }

        var appointment = await _appointmentRepository.GetByPublicIdAsync(appointmentId);
        if (appointment == null)
        {
            throw new NotFoundException(
                "Appointment not found",
                DomainErrorCodes.Appointment.NotFound
            );
        }

        var existingSession = await _sessionRepository.GetByAppointmentIdAsync(appointment.Id);
        if (existingSession != null)
        {
            throw new ConflictException(
                "Session already exists for this appointment",
                DomainErrorCodes.Session.AlreadyExistsForAppointment
            );
        }

        if (appointment.StudentId == null)
        {
            throw new ValidationException(
                "Appointment must have a linked student to start a session",
                DomainErrorCodes.Session.StudentNotLinked
            );
        }

        appointment.Status = AppointmentStatus.InSession;
        await _appointmentRepository.UpdateAsync(appointment);

        var now = DateTimeOffset.UtcNow;

        var student = await _userRepository.GetByIdAsync(appointment.StudentId.Value);
        if (student == null)
        {
            throw new NotFoundException("Student not found", DomainErrorCodes.User.NotFound);
        }

        var session = new Session
        {
            AppointmentId = appointment.Id,
            ProfessionalId = professional.Id,
            StudentId = student.Id,
            StartedAt = now,
            Notes = string.Empty,
            Status = SessionStatus.InProgress,
        };

        await _sessionRepository.AddAsync(session);

        session.Appointment = appointment;
        session.Professional = professional;
        session.Student = student;

        return MapToDto(session);
    }

    public async Task<SessionDto> UpdateAsync(Guid id, UpdateSessionDto dto)
    {
        var session = await _sessionRepository.GetByPublicIdAsync(id);
        if (session == null)
        {
            throw new NotFoundException("Session not found", DomainErrorCodes.Session.NotFound);
        }

        if (dto.EndedAt.HasValue)
        {
            session.EndedAt = dto.EndedAt.Value;
        }

        if (dto.Notes != null)
        {
            session.Notes = dto.Notes;
        }

        if (dto.Status != null)
        {
            if (Enum.TryParse<SessionStatus>(dto.Status, true, out var status))
            {
                session.Status = status;

                if (status == SessionStatus.Completed || status == SessionStatus.Cancelled)
                {
                    var appointment = await _appointmentRepository.GetByIdAsync(
                        session.AppointmentId
                    );
                    if (appointment != null)
                    {
                        appointment.Status =
                            status == SessionStatus.Completed
                                ? AppointmentStatus.Completed
                                : AppointmentStatus.Cancelled;
                        await _appointmentRepository.UpdateAsync(appointment);
                    }
                }
            }
        }

        await _sessionRepository.UpdateAsync(session);

        return MapToDto(session);
    }

    public async Task DeleteAsync(Guid id)
    {
        var session = await _sessionRepository.GetByPublicIdAsync(id);
        if (session == null)
        {
            throw new NotFoundException("Session not found", DomainErrorCodes.Session.NotFound);
        }

        await _sessionRepository.DeleteAsync(session.Id);
    }

    private static SessionDto MapToDto(Session session)
    {
        return new SessionDto
        {
            Id = session.PublicId,
            AppointmentId = session.Appointment?.PublicId ?? Guid.Empty,
            ProfessionalId = session.Professional?.PublicId ?? Guid.Empty,
            ProfessionalName = session.Professional?.Name ?? string.Empty,
            StudentId = session.Student?.PublicId ?? Guid.Empty,
            StudentName = session.Student?.Name ?? string.Empty,
            StartedAt = session.StartedAt,
            EndedAt = session.EndedAt,
            Notes = session.Notes,
            Status = session.Status.ToString(),
            CreatedAt = session.CreatedAt,
            UpdatedAt = session.UpdatedAt,
        };
    }
}
