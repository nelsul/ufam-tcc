using IcompCare.Application.DTOs;
using IcompCare.Application.DTOs.PatientRecords;

namespace IcompCare.Application.Interfaces;

public interface IPatientRecordService
{
    Task<PagedResult<PatientRecordDto>> GetAllAsync(int pageNumber, int pageSize);
    Task<PatientRecordDto> GetByIdAsync(Guid id);
    Task<IEnumerable<PatientRecordDto>> GetByStudentIdAsync(Guid studentId);
    Task<PatientRecordDto> CreateAsync(CreatePatientRecordDto dto);
    Task<PatientRecordDto> UpdateAsync(Guid id, UpdatePatientRecordDto dto);
    Task DeleteAsync(Guid id);
}
