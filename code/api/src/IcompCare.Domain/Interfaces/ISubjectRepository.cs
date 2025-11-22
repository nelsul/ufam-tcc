using IcompCare.Domain.Entities;

namespace IcompCare.Domain.Interfaces;

public interface ISubjectRepository
{
    Task<IEnumerable<Subject>> GetAllAsync(bool includeInactive = false);
    Task<(IEnumerable<Subject> Items, int TotalCount)> GetAllAsync(
        int pageNumber,
        int pageSize,
        bool includeInactive = false,
        string? search = null
    );
    Task<Subject?> GetByIdAsync(long id);
    Task<Subject?> GetByPublicIdAsync(Guid publicId);
    Task<Subject?> GetByCodeAsync(string code);
    Task<Subject> AddAsync(Subject subject);
    Task UpdateAsync(Subject subject);
    Task DeleteAsync(long id);
    Task<IEnumerable<Subject>> GetActiveSubjectsAsync(string? search = null);
}
