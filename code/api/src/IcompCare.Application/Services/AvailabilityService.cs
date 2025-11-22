using IcompCare.Application.DTOs;
using IcompCare.Application.DTOs.Availabilities;
using IcompCare.Application.DTOs.Users;
using IcompCare.Application.Interfaces;
using IcompCare.Domain.Constants;
using IcompCare.Domain.Entities;
using IcompCare.Domain.Enums;
using IcompCare.Domain.Exceptions;
using IcompCare.Domain.Interfaces;

namespace IcompCare.Application.Services;

public class AvailabilityService : IAvailabilityService
{
    private const int MinimumAppointmentDurationMinutes = 30;

    private readonly IAvailabilityRepository _availabilityRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAppointmentRepository _appointmentRepository;

    public AvailabilityService(
        IAvailabilityRepository availabilityRepository,
        IUserRepository userRepository,
        IAppointmentRepository appointmentRepository
    )
    {
        _availabilityRepository = availabilityRepository;
        _userRepository = userRepository;
        _appointmentRepository = appointmentRepository;
    }

    public async Task<PagedResult<AvailabilityDto>> GetAllAvailabilitiesAsync(
        int pageNumber,
        int pageSize,
        bool includeInactive = false
    )
    {
        var (availabilities, totalCount) = await _availabilityRepository.GetAllAsync(
            pageNumber,
            pageSize,
            includeInactive
        );

        var availabilityDtos = availabilities.Select(MapToDto);

        return new PagedResult<AvailabilityDto>
        {
            Items = availabilityDtos,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
        };
    }

    public async Task<AvailabilityDto?> GetAvailabilityByIdAsync(Guid id)
    {
        var availability = await _availabilityRepository.GetByPublicIdAsync(id);
        if (availability == null)
            return null;

        return MapToDto(availability);
    }

    public async Task<AvailabilityDto> CreateAvailabilityAsync(
        CreateAvailabilityDto createAvailabilityDto
    )
    {
        if (createAvailabilityDto.EndTime <= createAvailabilityDto.StartTime)
        {
            throw new ValidationException(
                "End time must be greater than start time.",
                DomainErrorCodes.Availability.InvalidTimeRange
            );
        }

        var professional = await _userRepository.GetByPublicIdAsync(
            createAvailabilityDto.ProfessionalId
        );
        if (professional == null)
        {
            throw new NotFoundException("User", createAvailabilityDto.ProfessionalId);
        }

        var hasOverlap = await _availabilityRepository.HasOverlapAsync(
            professional.Id,
            createAvailabilityDto.StartTime,
            createAvailabilityDto.EndTime
        );
        if (hasOverlap)
        {
            throw new ConflictException(
                "The availability overlaps with an existing one.",
                DomainErrorCodes.Availability.Overlap
            );
        }

        var availability = new Availability
        {
            ProfessionalId = professional.Id,
            StartTime = createAvailabilityDto.StartTime,
            EndTime = createAvailabilityDto.EndTime,
            Professional = professional,
        };

        var createdAvailability = await _availabilityRepository.AddAsync(availability);

        createdAvailability.Professional = professional;

        return MapToDto(createdAvailability);
    }

    public async Task UpdateAvailabilityAsync(Guid id, UpdateAvailabilityDto updateAvailabilityDto)
    {
        if (updateAvailabilityDto.EndTime <= updateAvailabilityDto.StartTime)
        {
            throw new ValidationException(
                "End time must be greater than start time.",
                DomainErrorCodes.Availability.InvalidTimeRange
            );
        }

        var availability = await _availabilityRepository.GetByPublicIdAsync(id);
        if (availability == null)
        {
            throw new NotFoundException("Availability", id);
        }

        var hasOverlap = await _availabilityRepository.HasOverlapAsync(
            availability.ProfessionalId,
            updateAvailabilityDto.StartTime,
            updateAvailabilityDto.EndTime,
            id
        );
        if (hasOverlap)
        {
            throw new ConflictException(
                "The availability overlaps with an existing one.",
                DomainErrorCodes.Availability.Overlap
            );
        }

        availability.StartTime = updateAvailabilityDto.StartTime;
        availability.EndTime = updateAvailabilityDto.EndTime;
        availability.Status = updateAvailabilityDto.Status;

        await _availabilityRepository.UpdateAsync(availability);
    }

    public async Task DeleteAvailabilityAsync(Guid id)
    {
        var availability = await _availabilityRepository.GetByPublicIdAsync(id);
        if (availability == null)
        {
            throw new NotFoundException("Availability", id);
        }

        await _availabilityRepository.DeleteAsync(availability.Id);
    }

    public async Task<IEnumerable<ProfessionalAvailabilityDto>> GetProfessionalAvailabilityAsync(
        Guid professionalId
    )
    {
        var professional = await _userRepository.GetByPublicIdAsync(professionalId);
        if (professional == null)
        {
            throw new NotFoundException("User", professionalId);
        }

        var availabilities = await _availabilityRepository.GetByProfessionalIdAsync(
            professional.Id
        );
        var appointments = await _appointmentRepository.GetAllByProfessionalIdAsync(
            professional.Id
        );

        Console.WriteLine(
            $"[AvailabilityService] Professional ID: {professional.Id}, PublicId: {professionalId}"
        );
        Console.WriteLine(
            $"[AvailabilityService] Found {availabilities.Count()} availabilities, {appointments.Count()} appointments"
        );
        foreach (var avail in availabilities)
        {
            Console.WriteLine(
                $"[AvailabilityService] Availability: Id={avail.Id}, Status={avail.Status}, StartTime={avail.StartTime:O}, EndTime={avail.EndTime:O}"
            );
        }
        foreach (var appt in appointments)
        {
            Console.WriteLine(
                $"[AvailabilityService] Appointment: Id={appt.Id}, Status={appt.Status}, StartTime={appt.StartTime:O}, EndTime={appt.EndTime?.ToString("O") ?? "null"}"
            );
        }

        var now = DateTimeOffset.UtcNow;
        Console.WriteLine($"[AvailabilityService] Current UTC time: {now:O}");

        var result = new List<ProfessionalAvailabilityDto>();
        var groupedAvailabilities = availabilities
            .Where(a => a.Status == GeneralStatus.Active && a.EndTime > now)
            .GroupBy(a => a.StartTime.UtcDateTime.Date);

        foreach (var group in groupedAvailabilities)
        {
            var date = group.Key;
            var dayDto = new ProfessionalAvailabilityDto
            {
                DayOfWeek = date.DayOfWeek,
                Date = date.ToString("yyyy-MM-dd"),
                TimeRanges = new List<TimeRangeDto>(),
            };

            foreach (var availability in group)
            {
                var availabilityStart = availability.StartTime;
                var availabilityEnd = availability.EndTime;

                Console.WriteLine(
                    $"[AvailabilityService] Processing availability: {availabilityStart:O} - {availabilityEnd:O}"
                );

                var freeRanges = new List<(DateTimeOffset Start, DateTimeOffset End)>
                {
                    (availabilityStart, availabilityEnd),
                };

                var overlappingAppointments = appointments
                    .Where(app =>
                    {
                        if (app.Status == AppointmentStatus.Cancelled)
                            return false;

                        var appEnd =
                            app.EndTime
                            ?? app.StartTime.AddMinutes(MinimumAppointmentDurationMinutes);

                        var overlaps =
                            app.StartTime.UtcDateTime < availabilityEnd.UtcDateTime
                            && appEnd.UtcDateTime > availabilityStart.UtcDateTime;

                        Console.WriteLine(
                            $"[AvailabilityService] Checking appointment {app.Id}: {app.StartTime:O} - {appEnd:O}, overlaps={overlaps}"
                        );

                        return overlaps;
                    })
                    .OrderBy(app => app.StartTime)
                    .ToList();

                Console.WriteLine(
                    $"[AvailabilityService] Found {overlappingAppointments.Count} overlapping appointments"
                );

                foreach (var appt in overlappingAppointments)
                {
                    var apptEndTime =
                        appt.EndTime
                        ?? appt.StartTime.AddMinutes(MinimumAppointmentDurationMinutes);
                    var newFreeRanges = new List<(DateTimeOffset Start, DateTimeOffset End)>();

                    var apptStartUtc = appt.StartTime.UtcDateTime;
                    var apptEndUtc = apptEndTime.UtcDateTime;

                    foreach (var fr in freeRanges)
                    {
                        var frStartUtc = fr.Start.UtcDateTime;
                        var frEndUtc = fr.End.UtcDateTime;

                        if (apptEndUtc <= frStartUtc || apptStartUtc >= frEndUtc)
                        {
                            newFreeRanges.Add(fr);
                            continue;
                        }

                        if (apptStartUtc <= frStartUtc && apptEndUtc >= frEndUtc)
                        {
                            continue;
                        }

                        if (apptStartUtc <= frStartUtc && apptEndUtc < frEndUtc)
                        {
                            newFreeRanges.Add((apptEndTime, fr.End));
                            continue;
                        }

                        if (apptStartUtc > frStartUtc && apptEndUtc >= frEndUtc)
                        {
                            newFreeRanges.Add((fr.Start, appt.StartTime));
                            continue;
                        }

                        if (apptStartUtc > frStartUtc && apptEndUtc < frEndUtc)
                        {
                            newFreeRanges.Add((fr.Start, appt.StartTime));
                            newFreeRanges.Add((apptEndTime, fr.End));
                            continue;
                        }
                    }

                    freeRanges = newFreeRanges;
                    if (!freeRanges.Any())
                        break;
                }

                Console.WriteLine(
                    $"[AvailabilityService] After subtraction, {freeRanges.Count} free ranges:"
                );
                foreach (var fr in freeRanges)
                {
                    Console.WriteLine($"[AvailabilityService]   {fr.Start:O} - {fr.End:O}");
                }

                foreach (var fr in freeRanges)
                {
                    if (fr.Start < fr.End)
                    {
                        dayDto.TimeRanges.Add(new TimeRangeDto { Start = fr.Start, End = fr.End });
                    }
                }
            }

            if (dayDto.TimeRanges.Any())
            {
                result.Add(dayDto);
            }
        }

        return result.OrderBy(r => r.Date);
    }

    private static AvailabilityDto MapToDto(Availability availability)
    {
        return new AvailabilityDto
        {
            PublicId = availability.PublicId,
            StartTime = availability.StartTime,
            EndTime = availability.EndTime,
            Status = availability.Status,
            Professional = new UserDto
            {
                PublicId = availability.Professional.PublicId,
                Name = availability.Professional.Name,
                FullName = availability.Professional.FullName,
                InstitutionalEmail = availability.Professional.InstitutionalEmail,
                Registration = availability.Professional.Registration,
                Status = availability.Professional.Status,
                Role = availability.Professional.Role,
                CreatedAt = availability.Professional.CreatedAt,
                UpdatedAt = availability.Professional.UpdatedAt,
            },
            CreatedAt = availability.CreatedAt,
            UpdatedAt = availability.UpdatedAt,
        };
    }

    public async Task<IEnumerable<AvailabilityDto>> GetMyAvailabilitiesAsync(Guid userId)
    {
        var user = await _userRepository.GetByPublicIdAsync(userId);
        if (user == null)
            throw new NotFoundException("User", userId);

        var availabilities = await _availabilityRepository.GetByProfessionalIdAsync(user.Id);
        return availabilities.Select(MapToDto);
    }

    public async Task<AvailabilityDto> CreateMyAvailabilityAsync(
        Guid userId,
        CreateMyAvailabilityDto dto
    )
    {
        var user = await _userRepository.GetByPublicIdAsync(userId);
        if (user == null)
            throw new NotFoundException("User", userId);

        if (dto.EndTime <= dto.StartTime)
        {
            throw new ValidationException(
                "End time must be greater than start time.",
                DomainErrorCodes.Availability.InvalidTimeRange
            );
        }

        var hasOverlap = await _availabilityRepository.HasOverlapAsync(
            user.Id,
            dto.StartTime,
            dto.EndTime
        );
        if (hasOverlap)
        {
            throw new ConflictException(
                "The availability overlaps with an existing one.",
                DomainErrorCodes.Availability.Overlap
            );
        }

        var availability = new Availability
        {
            ProfessionalId = user.Id,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            Professional = user,
        };

        await _availabilityRepository.AddAsync(availability);
        return MapToDto(availability);
    }

    public async Task UpdateMyAvailabilityAsync(
        Guid userId,
        Guid availabilityId,
        UpdateAvailabilityDto dto
    )
    {
        var user = await _userRepository.GetByPublicIdAsync(userId);
        if (user == null)
            throw new NotFoundException("User", userId);

        var availability = await _availabilityRepository.GetByPublicIdAsync(availabilityId);
        if (availability == null)
            throw new NotFoundException("Availability", availabilityId);

        if (availability.ProfessionalId != user.Id)
        {
            throw new ForbiddenException("You can only update your own availabilities.");
        }

        if (dto.EndTime <= dto.StartTime)
        {
            throw new ValidationException(
                "End time must be greater than start time.",
                DomainErrorCodes.Availability.InvalidTimeRange
            );
        }

        var hasOverlap = await _availabilityRepository.HasOverlapAsync(
            user.Id,
            dto.StartTime,
            dto.EndTime,
            availability.PublicId
        );
        if (hasOverlap)
        {
            throw new ConflictException(
                "The availability overlaps with an existing one.",
                DomainErrorCodes.Availability.Overlap
            );
        }

        availability.StartTime = dto.StartTime;
        availability.EndTime = dto.EndTime;
        availability.Status = dto.Status;

        await _availabilityRepository.UpdateAsync(availability);
    }

    public async Task DeleteMyAvailabilityAsync(Guid userId, Guid availabilityId)
    {
        var user = await _userRepository.GetByPublicIdAsync(userId);
        if (user == null)
            throw new NotFoundException("User", userId);

        var availability = await _availabilityRepository.GetByPublicIdAsync(availabilityId);
        if (availability == null)
            throw new NotFoundException("Availability", availabilityId);

        if (availability.ProfessionalId != user.Id)
        {
            throw new ForbiddenException("You can only delete your own availabilities.");
        }

        await _availabilityRepository.DeleteAsync(availability.Id);
    }
}
