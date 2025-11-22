using IcompCare.Application.DTOs;
using IcompCare.Application.DTOs.Sessions;

namespace IcompCare.Application.Interfaces;

public interface ISessionService
{
    Task<PagedResult<SessionDto>> GetAllAsync(int pageNumber, int pageSize);
    Task<PagedResult<SessionDto>> GetByProfessionalAsync(
        Guid professionalId,
        int pageNumber,
        int pageSize,
        DateTimeOffset? dateFrom = null,
        DateTimeOffset? dateTo = null,
        string? search = null
    );
    Task<SessionDto?> GetOpenSessionByProfessionalAsync(Guid professionalId);
    Task<SessionDto> GetByIdAsync(Guid id);
    Task<SessionDto> CreateAsync(CreateSessionDto dto);
    Task<SessionDto> StartSessionAsync(Guid appointmentId, Guid professionalId);
    Task<SessionDto> UpdateAsync(Guid id, UpdateSessionDto dto);
    Task DeleteAsync(Guid id);
}
