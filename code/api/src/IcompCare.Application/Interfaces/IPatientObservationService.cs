using IcompCare.Application.DTOs;
using IcompCare.Application.DTOs.PatientObservations;

namespace IcompCare.Application.Interfaces;

public interface IPatientObservationService
{
    Task<PagedResult<PatientObservationDto>> GetAllAsync(int pageNumber, int pageSize);
    Task<PatientObservationDto> GetByIdAsync(Guid id);
    Task<IEnumerable<PatientObservationDto>> GetByStudentIdAsync(Guid studentId);
    Task<PatientObservationDto> CreateAsync(CreatePatientObservationDto dto);
    Task<PatientObservationDto> UpdateAsync(Guid id, UpdatePatientObservationDto dto);
    Task DeleteAsync(Guid id);
}
