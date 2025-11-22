using IcompCare.Application.DTOs;
using IcompCare.Application.DTOs.SubjectOfferings;

namespace IcompCare.Application.Interfaces;

public interface ISubjectOfferingService
{
    Task<PagedResult<SubjectOfferingDto>> GetAllSubjectOfferingsAsync(
        int pageNumber,
        int pageSize,
        bool includeInactive = false,
        string? search = null
    );
    Task<SubjectOfferingDto?> GetSubjectOfferingByIdAsync(Guid id);
    Task<SubjectOfferingDto> CreateSubjectOfferingAsync(
        CreateSubjectOfferingDto createSubjectOfferingDto
    );
    Task UpdateSubjectOfferingAsync(Guid id, UpdateSubjectOfferingDto updateSubjectOfferingDto);
    Task DeleteSubjectOfferingAsync(Guid id);
    Task<PagedResult<SubjectOfferingDto>> GetActiveOfferingsByDateAsync(
        DateOnly date,
        int pageNumber,
        int pageSize
    );
    Task<PagedResult<SubjectOfferingDto>> GetByProfessorIdAsync(
        Guid professorPublicId,
        int pageNumber,
        int pageSize,
        bool includeInactive = false,
        string? search = null
    );
}
