using IcompCare.Application.DTOs;
using IcompCare.Application.DTOs.Subjects;

namespace IcompCare.Application.Interfaces;

public interface ISubjectService
{
    Task<PagedResult<SubjectDto>> GetAllSubjectsAsync(
        int pageNumber,
        int pageSize,
        bool includeInactive = false,
        string? search = null
    );
    Task<SubjectDto?> GetSubjectByIdAsync(Guid id);
    Task<SubjectDto> CreateSubjectAsync(CreateSubjectDto createSubjectDto);
    Task UpdateSubjectAsync(Guid id, UpdateSubjectDto updateSubjectDto);
    Task DeleteSubjectAsync(Guid id);
    Task<IEnumerable<SubjectDto>> GetActiveSubjectsAsync(string? search = null);
}
