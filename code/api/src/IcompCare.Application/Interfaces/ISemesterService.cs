using IcompCare.Application.DTOs;
using IcompCare.Application.DTOs.Semesters;

namespace IcompCare.Application.Interfaces;

public interface ISemesterService
{
    Task<PagedResult<SemesterDto>> GetAllSemestersAsync(
        int pageNumber,
        int pageSize,
        bool includeInactive = false,
        string? search = null
    );
    Task<SemesterDto?> GetSemesterByIdAsync(Guid id);
    Task<SemesterDto> CreateSemesterAsync(CreateSemesterDto createSemesterDto);
    Task UpdateSemesterAsync(Guid id, UpdateSemesterDto updateSemesterDto);
    Task DeleteSemesterAsync(Guid id);
    Task<SemesterDto?> GetCurrentSemesterAsync();
}
