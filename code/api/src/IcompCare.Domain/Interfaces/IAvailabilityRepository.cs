using IcompCare.Domain.Entities;

namespace IcompCare.Domain.Interfaces;

public interface IAvailabilityRepository
{
    Task<IEnumerable<Availability>> GetAllAsync(bool includeInactive = false);
    Task<(IEnumerable<Availability> Items, int TotalCount)> GetAllAsync(
        int pageNumber,
        int pageSize,
        bool includeInactive = false
    );
    Task<Availability?> GetByIdAsync(long id);
    Task<Availability?> GetByPublicIdAsync(Guid publicId);
    Task<IEnumerable<Availability>> GetByProfessionalIdAsync(long professionalId);
    Task<bool> HasOverlapAsync(
        long professionalId,
        DateTimeOffset startTime,
        DateTimeOffset endTime,
        Guid? excludePublicId = null
    );
    Task<Availability> AddAsync(Availability availability);
    Task UpdateAsync(Availability availability);
    Task DeleteAsync(long id);
}
