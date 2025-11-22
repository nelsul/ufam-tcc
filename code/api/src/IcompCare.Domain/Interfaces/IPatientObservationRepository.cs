using IcompCare.Domain.Entities;

namespace IcompCare.Domain.Interfaces;

public interface IPatientObservationRepository
{
    Task<(IEnumerable<PatientObservation> Items, int TotalCount)> GetAllAsync(
        int pageNumber,
        int pageSize
    );
    Task<PatientObservation?> GetByIdAsync(long id);
    Task<PatientObservation?> GetByPublicIdAsync(Guid publicId);
    Task<IEnumerable<PatientObservation>> GetByStudentIdAsync(long studentId);
    Task<PatientObservation> AddAsync(PatientObservation patientObservation);
    Task UpdateAsync(PatientObservation patientObservation);
    Task DeleteAsync(long id);
}
