using IcompCare.Domain.Entities;

namespace IcompCare.Domain.Interfaces;

public interface IPatientRecordRepository
{
    Task<(IEnumerable<PatientRecord> Items, int TotalCount)> GetAllAsync(
        int pageNumber,
        int pageSize
    );
    Task<PatientRecord?> GetByIdAsync(long id);
    Task<PatientRecord?> GetByPublicIdAsync(Guid publicId);
    Task<IEnumerable<PatientRecord>> GetByStudentIdAsync(long studentId);
    Task<PatientRecord> AddAsync(PatientRecord patientRecord);
    Task UpdateAsync(PatientRecord patientRecord);
    Task DeleteAsync(long id);
}
