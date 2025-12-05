using IcompCare.Application.DTOs;
using IcompCare.Application.DTOs.Appointments;
using IcompCare.Application.DTOs.Email;
using IcompCare.Application.Interfaces;
using IcompCare.Domain.Constants;
using IcompCare.Domain.Entities;
using IcompCare.Domain.Enums;
using IcompCare.Domain.Exceptions;
using IcompCare.Domain.Interfaces;

namespace IcompCare.Application.Services;

public class AppointmentService : IAppointmentService
{
    private const int MinimumAppointmentDurationMinutes = 30;

    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IUserRepository _userRepository;
    private readonly ISessionTypeRepository _sessionTypeRepository;
    private readonly IAvailabilityRepository _availabilityRepository;
    private readonly IEmailQueueService _emailQueueService;
    private readonly IEmailTemplateService _emailTemplateService;

    public AppointmentService(
        IAppointmentRepository appointmentRepository,
        IUserRepository userRepository,
        ISessionTypeRepository sessionTypeRepository,
        IAvailabilityRepository availabilityRepository,
        IEmailQueueService emailQueueService,
        IEmailTemplateService emailTemplateService
    )
    {
        _appointmentRepository = appointmentRepository;
        _userRepository = userRepository;
        _sessionTypeRepository = sessionTypeRepository;
        _availabilityRepository = availabilityRepository;
        _emailQueueService = emailQueueService;
        _emailTemplateService = emailTemplateService;
    }

    public async Task<PagedResult<AppointmentDto>> GetAllAsync(int pageNumber, int pageSize)
    {
        var (items, totalCount) = await _appointmentRepository.GetAllAsync(pageNumber, pageSize);

        var dtos = items.Select(MapToDto).ToList();

        return new PagedResult<AppointmentDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
        };
    }

    public async Task<AppointmentDto> GetByIdAsync(Guid id)
    {
        var appointment = await _appointmentRepository.GetByPublicIdAsync(id);
        if (appointment == null)
        {
            throw new NotFoundException(
                "Appointment not found",
                DomainErrorCodes.Appointment.NotFound
            );
        }

        return MapToDto(appointment);
    }

    public async Task<AppointmentDto> CreateAsync(CreateAppointmentDto dto)
    {
        var professional = await _userRepository.GetByPublicIdAsync(dto.ProfessionalId);
        if (professional == null)
        {
            throw new NotFoundException("Professional not found", DomainErrorCodes.User.NotFound);
        }

        var tentativeEndTime = dto.StartTime.AddMinutes(MinimumAppointmentDurationMinutes);
        var hasOverlap = await _appointmentRepository.HasOverlapAsync(
            professional.Id,
            dto.StartTime,
            tentativeEndTime
        );

        if (hasOverlap)
        {
            throw new ConflictException(
                "This time slot is already booked. Please choose another time.",
                DomainErrorCodes.Appointment.Overlap
            );
        }

        var appointment = new Appointment
        {
            ProfessionalId = professional.Id,
            StudentEmail = dto.StudentEmail,
            StudentRegistration = dto.StudentRegistration,
            StudentFullName = dto.StudentFullName,
            SessionTypeId = null,
            StartTime = dto.StartTime,
            EndTime = null,
            ReasonForVisit = dto.ReasonForVisit,
            Status = AppointmentStatus.Pending,
        };

        await _appointmentRepository.AddAsync(appointment);

        appointment.Professional = professional;

        SendAppointmentRequestedEmailToStudentAsync(appointment);

        return MapToDto(appointment);
    }

    private static readonly TimeZoneInfo ManausTimeZone = TimeZoneInfo.FindSystemTimeZoneById(
        "America/Manaus"
    );

    private static DateTimeOffset ToManausTime(DateTimeOffset dateTime)
    {
        return TimeZoneInfo.ConvertTime(dateTime, ManausTimeZone);
    }

    private void SendAppointmentRequestedEmailToStudentAsync(Appointment appointment)
    {
        try
        {
            var manausTime = ToManausTime(appointment.StartTime);
            var templateModel = new
            {
                StudentName = appointment.StudentFullName,
                ProfessionalName = appointment.Professional?.FullName ?? "Profissional",
                AppointmentDate = manausTime.ToString("dd/MM/yyyy"),
                AppointmentTime = manausTime.ToString("HH:mm"),
                Reason = appointment.ReasonForVisit ?? string.Empty,
            };

            var bodyContent = _emailTemplateService.RenderTemplate(
                "StudentAppointmentRequested",
                templateModel
            );
            var htmlBody = _emailTemplateService.GetBaseLayout(
                bodyContent,
                "Solicitação de Agendamento Recebida"
            );

            var emailMessage = new EmailMessage
            {
                To = appointment.StudentEmail,
                Subject = "IcompCare - Solicitação de Agendamento Recebida",
                HtmlBody = htmlBody,
            };

            _emailQueueService.QueueEmail(emailMessage);
        }
        catch { }
    }

    public async Task<AppointmentDto> UpdateAsync(Guid id, UpdateAppointmentDto dto)
    {
        var appointment = await _appointmentRepository.GetByPublicIdAsync(id);
        if (appointment == null)
        {
            throw new NotFoundException(
                "Appointment not found",
                DomainErrorCodes.Appointment.NotFound
            );
        }

        var previousStatus = appointment.Status;
        var previousStartTime = appointment.StartTime;
        var previousEndTime = appointment.EndTime;

        if (dto.StudentId.HasValue)
        {
            var student = await _userRepository.GetByPublicIdAsync(dto.StudentId.Value);
            if (student == null || student.Role != UserRole.Student)
            {
                throw new NotFoundException("Student not found", DomainErrorCodes.User.NotFound);
            }
            appointment.StudentId = student.Id;
            appointment.Student = student;
        }

        if (dto.SessionTypeId.HasValue)
        {
            var sessionType = await _sessionTypeRepository.GetByPublicIdAsync(
                dto.SessionTypeId.Value
            );
            if (sessionType == null)
            {
                throw new NotFoundException(
                    "SessionType not found",
                    DomainErrorCodes.SessionType.NotFound
                );
            }
            appointment.SessionTypeId = sessionType.Id;
            appointment.SessionType = sessionType;
            appointment.EndTime = appointment.StartTime.AddMinutes(sessionType.DurationMinutes);
        }

        if (dto.StartTime.HasValue)
        {
            DateTimeOffset? newEndTime = null;
            if (appointment.SessionTypeId.HasValue && appointment.SessionType != null)
            {
                newEndTime = dto.StartTime.Value.AddMinutes(
                    appointment.SessionType.DurationMinutes
                );
            }
            else if (appointment.EndTime.HasValue)
            {
                var duration = appointment.EndTime.Value - appointment.StartTime;
                newEndTime = dto.StartTime.Value.Add(duration);
            }

            if (newEndTime.HasValue)
            {
                var hasOverlap = await _appointmentRepository.HasOverlapAsync(
                    appointment.ProfessionalId,
                    dto.StartTime.Value,
                    newEndTime.Value,
                    appointment.PublicId
                );

                if (hasOverlap)
                {
                    throw new ConflictException(
                        "Appointment overlaps with an existing one",
                        DomainErrorCodes.Appointment.Overlap
                    );
                }
                appointment.EndTime = newEndTime;
            }

            appointment.StartTime = dto.StartTime.Value;
        }

        if (dto.ReasonForVisit != null)
        {
            appointment.ReasonForVisit = dto.ReasonForVisit;
        }

        if (dto.EndTime.HasValue)
        {
            if (dto.EndTime.Value <= appointment.StartTime)
            {
                throw new ValidationException(
                    "End time must be after start time",
                    DomainErrorCodes.Appointment.InvalidTimeRange
                );
            }

            var hasOverlap = await _appointmentRepository.HasOverlapAsync(
                appointment.ProfessionalId,
                appointment.StartTime,
                dto.EndTime.Value,
                appointment.PublicId
            );

            if (hasOverlap)
            {
                throw new ConflictException(
                    "Appointment overlaps with an existing one",
                    DomainErrorCodes.Appointment.Overlap
                );
            }

            appointment.EndTime = dto.EndTime.Value;
        }

        if (dto.Status != null)
        {
            if (Enum.TryParse<AppointmentStatus>(dto.Status, true, out var status))
            {
                if (
                    status == AppointmentStatus.Confirmed
                    && appointment.Status != AppointmentStatus.Confirmed
                )
                {
                    var endTimeForCheck =
                        appointment.EndTime
                        ?? appointment.StartTime.AddMinutes(MinimumAppointmentDurationMinutes);

                    var hasOverlap = await _appointmentRepository.HasOverlapAsync(
                        appointment.ProfessionalId,
                        appointment.StartTime,
                        endTimeForCheck,
                        appointment.PublicId
                    );

                    if (hasOverlap)
                    {
                        throw new ConflictException(
                            "Cannot confirm: this time slot overlaps with another appointment.",
                            DomainErrorCodes.Appointment.Overlap
                        );
                    }

                    var availabilities = await _availabilityRepository.GetByProfessionalIdAsync(
                        appointment.ProfessionalId
                    );
                    var hasAvailability = availabilities.Any(a =>
                        a.Status == GeneralStatus.Active
                        && a.StartTime <= appointment.StartTime
                        && a.EndTime >= endTimeForCheck
                    );

                    if (!hasAvailability)
                    {
                        throw new ValidationException(
                            "Cannot confirm: no availability set for this time slot.",
                            DomainErrorCodes.Appointment.NoAvailability
                        );
                    }
                }

                appointment.Status = status;
            }
        }

        await _appointmentRepository.UpdateAsync(appointment);

        SendAppointmentUpdateEmailIfNeeded(
            appointment,
            previousStatus,
            previousStartTime,
            previousEndTime
        );

        return MapToDto(appointment);
    }

    private void SendAppointmentUpdateEmailIfNeeded(
        Appointment appointment,
        AppointmentStatus previousStatus,
        DateTimeOffset previousStartTime,
        DateTimeOffset? previousEndTime
    )
    {
        try
        {
            var statusChanged = appointment.Status != previousStatus;
            var startTimeChanged = appointment.StartTime != previousStartTime;
            var endTimeChanged = appointment.EndTime != previousEndTime;

            if (
                statusChanged
                && previousStatus == AppointmentStatus.Pending
                && appointment.Status == AppointmentStatus.Confirmed
            )
            {
                SendAppointmentConfirmedEmail(appointment);
            }
            else if (statusChanged && appointment.Status == AppointmentStatus.Cancelled)
            {
                SendAppointmentCancelledEmail(appointment);
            }
            else if (
                (startTimeChanged || endTimeChanged)
                && appointment.Status != AppointmentStatus.Cancelled
            )
            {
                SendAppointmentRescheduledEmail(appointment, previousStartTime, previousEndTime);
            }
        }
        catch { }
    }

    private void SendAppointmentConfirmedEmail(Appointment appointment)
    {
        if (string.IsNullOrEmpty(appointment.StudentEmail))
            return;

        var manausStartTime = ToManausTime(appointment.StartTime);
        var duration = appointment.EndTime.HasValue
            ? (int)(appointment.EndTime.Value - appointment.StartTime).TotalMinutes
            : MinimumAppointmentDurationMinutes;

        var templateModel = new
        {
            PatientName = appointment.StudentFullName,
            ProfessionalName = appointment.Professional?.FullName ?? "Profissional",
            AppointmentDate = manausStartTime.ToString("dd/MM/yyyy"),
            AppointmentTime = manausStartTime.ToString("HH:mm"),
            Duration = duration.ToString(),
        };

        var bodyContent = _emailTemplateService.RenderTemplate(
            "AppointmentConfirmed",
            templateModel
        );
        var htmlBody = _emailTemplateService.GetBaseLayout(bodyContent, "Agendamento Confirmado");

        var emailMessage = new EmailMessage
        {
            To = appointment.StudentEmail,
            Subject = "IcompCare - Agendamento Confirmado",
            HtmlBody = htmlBody,
        };

        _emailQueueService.QueueEmail(emailMessage);
    }

    private void SendAppointmentCancelledEmail(Appointment appointment)
    {
        if (string.IsNullOrEmpty(appointment.StudentEmail))
            return;

        var manausStartTime = ToManausTime(appointment.StartTime);

        var templateModel = new
        {
            RecipientName = appointment.StudentFullName,
            AppointmentDate = manausStartTime.ToString("dd/MM/yyyy"),
            AppointmentTime = manausStartTime.ToString("HH:mm"),
            CancellationReason = string.Empty,
        };

        var bodyContent = _emailTemplateService.RenderTemplate(
            "AppointmentCancelled",
            templateModel
        );
        var htmlBody = _emailTemplateService.GetBaseLayout(bodyContent, "Agendamento Cancelado");

        var emailMessage = new EmailMessage
        {
            To = appointment.StudentEmail,
            Subject = "IcompCare - Agendamento Cancelado",
            HtmlBody = htmlBody,
        };

        _emailQueueService.QueueEmail(emailMessage);
    }

    private void SendAppointmentRescheduledEmail(
        Appointment appointment,
        DateTimeOffset previousStartTime,
        DateTimeOffset? previousEndTime
    )
    {
        if (string.IsNullOrEmpty(appointment.StudentEmail))
            return;

        var manausOldStartTime = ToManausTime(previousStartTime);
        var manausNewStartTime = ToManausTime(appointment.StartTime);

        var duration = appointment.EndTime.HasValue
            ? (int)(appointment.EndTime.Value - appointment.StartTime).TotalMinutes
            : MinimumAppointmentDurationMinutes;

        var templateModel = new
        {
            PatientName = appointment.StudentFullName,
            ProfessionalName = appointment.Professional?.FullName ?? "Profissional",
            OldAppointmentDate = manausOldStartTime.ToString("dd/MM/yyyy"),
            OldAppointmentTime = manausOldStartTime.ToString("HH:mm"),
            NewAppointmentDate = manausNewStartTime.ToString("dd/MM/yyyy"),
            NewAppointmentTime = manausNewStartTime.ToString("HH:mm"),
            Duration = duration.ToString(),
        };

        var bodyContent = _emailTemplateService.RenderTemplate(
            "AppointmentRescheduled",
            templateModel
        );
        var htmlBody = _emailTemplateService.GetBaseLayout(bodyContent, "Agendamento Remarcado");

        var emailMessage = new EmailMessage
        {
            To = appointment.StudentEmail,
            Subject = "IcompCare - Agendamento Remarcado",
            HtmlBody = htmlBody,
        };

        _emailQueueService.QueueEmail(emailMessage);
    }

    public async Task DeleteAsync(Guid id)
    {
        var appointment = await _appointmentRepository.GetByPublicIdAsync(id);
        if (appointment == null)
        {
            throw new NotFoundException(
                "Appointment not found",
                DomainErrorCodes.Appointment.NotFound
            );
        }

        await _appointmentRepository.DeleteAsync(appointment.Id);
    }

    public async Task<PagedResult<AppointmentDto>> GetByProfessionalIdAsync(
        Guid professionalId,
        int pageNumber = 1,
        int pageSize = 20,
        string? status = null,
        Guid? sessionTypeId = null,
        string? search = null
    )
    {
        var professional = await _userRepository.GetByPublicIdAsync(professionalId);
        if (professional == null)
        {
            throw new NotFoundException("Professional not found", DomainErrorCodes.User.NotFound);
        }

        var (appointments, totalCount) = await _appointmentRepository.GetByProfessionalIdAsync(
            professional.Id,
            pageNumber,
            pageSize,
            status,
            sessionTypeId,
            search
        );

        return new PagedResult<AppointmentDto>
        {
            Items = appointments.Select(MapToDto),
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
        };
    }

    private static AppointmentDto MapToDto(Appointment appointment)
    {
        return new AppointmentDto
        {
            Id = appointment.PublicId,
            ProfessionalId = appointment.Professional?.PublicId ?? Guid.Empty,
            ProfessionalName = appointment.Professional?.Name ?? string.Empty,
            StudentId = appointment.Student?.PublicId,
            StudentEmail = appointment.StudentEmail,
            StudentRegistration = appointment.StudentRegistration,
            StudentFullName = appointment.StudentFullName,
            SessionTypeId = appointment.SessionType?.PublicId,
            SessionTypeName = appointment.SessionType?.Name,
            StartTime = appointment.StartTime,
            EndTime = appointment.EndTime,
            Status = appointment.Status.ToString(),
            ReasonForVisit = appointment.ReasonForVisit,
            CreatedAt = appointment.CreatedAt,
            UpdatedAt = appointment.UpdatedAt,
        };
    }
}
