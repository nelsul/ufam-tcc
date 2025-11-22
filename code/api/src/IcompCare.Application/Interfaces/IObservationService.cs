using IcompCare.Application.DTOs;
using IcompCare.Application.DTOs.Observations;

namespace IcompCare.Application.Interfaces;

public interface IObservationService
{
    Task<PagedResult<ObservationDto>> GetAllAsync(
        int pageNumber,
        int pageSize,
        bool includeInactive = false,
        string? search = null
    );
    Task<ObservationDto> GetByIdAsync(Guid id);
    Task<ObservationDto> CreateAsync(CreateObservationDto dto);
    Task<ObservationDto> UpdateAsync(Guid id, UpdateObservationDto dto);
    Task DeleteAsync(Guid id);
}
