using IcompCare.Application.DTOs;
using IcompCare.Application.DTOs.SessionTypes;

namespace IcompCare.Application.Interfaces;

public interface ISessionTypeService
{
    Task<PagedResult<SessionTypeDto>> GetAllSessionTypesAsync(
        int pageNumber,
        int pageSize,
        bool includeInactive = false,
        string? search = null
    );
    Task<SessionTypeDto?> GetSessionTypeByIdAsync(Guid id);
    Task<SessionTypeDto> CreateSessionTypeAsync(CreateSessionTypeDto createSessionTypeDto);
    Task UpdateSessionTypeAsync(Guid id, UpdateSessionTypeDto updateSessionTypeDto);
    Task DeleteSessionTypeAsync(Guid id);
}
