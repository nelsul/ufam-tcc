using IcompCare.Application.DTOs;
using IcompCare.Application.DTOs.Availabilities;

namespace IcompCare.Application.Interfaces;

public interface IAvailabilityService
{
    Task<PagedResult<AvailabilityDto>> GetAllAvailabilitiesAsync(
        int pageNumber,
        int pageSize,
        bool includeInactive = false
    );
    Task<AvailabilityDto?> GetAvailabilityByIdAsync(Guid id);
    Task<AvailabilityDto> CreateAvailabilityAsync(CreateAvailabilityDto createAvailabilityDto);
    Task UpdateAvailabilityAsync(Guid id, UpdateAvailabilityDto updateAvailabilityDto);
    Task DeleteAvailabilityAsync(Guid id);
    Task<IEnumerable<ProfessionalAvailabilityDto>> GetProfessionalAvailabilityAsync(
        Guid professionalId
    );
    Task<IEnumerable<AvailabilityDto>> GetMyAvailabilitiesAsync(Guid userId);
    Task<AvailabilityDto> CreateMyAvailabilityAsync(Guid userId, CreateMyAvailabilityDto dto);
    Task UpdateMyAvailabilityAsync(Guid userId, Guid availabilityId, UpdateAvailabilityDto dto);
    Task DeleteMyAvailabilityAsync(Guid userId, Guid availabilityId);
}
