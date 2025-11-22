using IcompCare.Domain.Entities;

namespace IcompCare.Domain.Interfaces;

public interface ISubjectOfferingRepository
{
    Task<(IEnumerable<SubjectOffering> Items, int TotalCount)> GetAllAsync(
        int pageNumber,
        int pageSize,
        bool includeInactive = false,
        string? search = null
    );
    Task<SubjectOffering?> GetByPublicIdAsync(Guid publicId);
    Task<SubjectOffering?> GetByUniqueKeyAsync(long semesterId, long subjectId, long professorId);
    Task<SubjectOffering> AddAsync(SubjectOffering subjectOffering);
    Task UpdateAsync(SubjectOffering subjectOffering);
    Task DeleteAsync(long id);
    Task<(IEnumerable<SubjectOffering> Items, int TotalCount)> GetActiveOfferingsByDateAsync(
        DateOnly date,
        int pageNumber,
        int pageSize
    );
    Task<(IEnumerable<SubjectOffering> Items, int TotalCount)> GetByProfessorIdAsync(
        Guid professorPublicId,
        int pageNumber,
        int pageSize,
        bool includeInactive = false,
        string? search = null
    );
}
