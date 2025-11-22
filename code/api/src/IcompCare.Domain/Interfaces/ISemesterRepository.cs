using IcompCare.Domain.Entities;

namespace IcompCare.Domain.Interfaces;

public interface ISemesterRepository
{
    Task<IEnumerable<Semester>> GetAllAsync(bool includeInactive = false);
    Task<(IEnumerable<Semester> Items, int TotalCount)> GetAllAsync(
        int pageNumber,
        int pageSize,
        bool includeInactive = false,
        string? search = null
    );
    Task<Semester?> GetByIdAsync(long id);
    Task<Semester?> GetByPublicIdAsync(Guid publicId);
    Task<Semester?> GetByNameAsync(string name);
    Task<Semester?> GetByDateAsync(DateOnly date);
    Task<Semester> AddAsync(Semester semester);
    Task UpdateAsync(Semester semester);
    Task DeleteAsync(long id);
}
