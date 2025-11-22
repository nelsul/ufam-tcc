using IcompCare.Domain.Entities;

namespace IcompCare.Domain.Interfaces;

public interface IObservationRepository
{
    Task<(IEnumerable<Observation> Items, int TotalCount)> GetAllAsync(
        int pageNumber,
        int pageSize,
        bool includeInactive = false,
        string? search = null
    );
    Task<Observation?> GetByIdAsync(long id);
    Task<Observation?> GetByPublicIdAsync(Guid publicId);
    Task<bool> ExistsByNameAsync(string name, Guid? excludePublicId = null);
    Task<Observation> AddAsync(Observation observation);
    Task UpdateAsync(Observation observation);
    Task DeleteAsync(long id);
}
