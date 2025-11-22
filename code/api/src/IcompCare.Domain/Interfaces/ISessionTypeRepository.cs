using IcompCare.Domain.Entities;

namespace IcompCare.Domain.Interfaces;

public interface ISessionTypeRepository
{
    Task<IEnumerable<SessionType>> GetAllAsync(bool includeInactive = false);
    Task<(IEnumerable<SessionType> Items, int TotalCount)> GetAllAsync(
        int pageNumber,
        int pageSize,
        bool includeInactive = false,
        string? search = null
    );
    Task<SessionType?> GetByIdAsync(long id);
    Task<SessionType?> GetByPublicIdAsync(Guid publicId);
    Task<SessionType?> GetByNameAsync(string name);
    Task<SessionType> AddAsync(SessionType sessionType);
    Task UpdateAsync(SessionType sessionType);
    Task DeleteAsync(long id);
}
