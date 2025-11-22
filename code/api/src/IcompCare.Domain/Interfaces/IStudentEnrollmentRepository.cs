using IcompCare.Domain.Entities;

namespace IcompCare.Domain.Interfaces;

public interface IStudentEnrollmentRepository
{
    Task<IEnumerable<StudentEnrollment>> GetAllAsync();
    Task<StudentEnrollment?> GetByIdAsync(long id);
    Task<StudentEnrollment?> GetByPublicIdAsync(Guid publicId);
    Task<StudentEnrollment> AddAsync(StudentEnrollment entity);
    Task UpdateAsync(StudentEnrollment entity);
    Task DeleteAsync(StudentEnrollment entity);
    Task<IEnumerable<StudentEnrollment>> GetByStudentIdAsync(long studentId);
    Task<IEnumerable<StudentEnrollment>> GetBySubjectOfferingIdAsync(long subjectOfferingId);
    Task<StudentEnrollment?> GetByStudentAndOfferingAsync(long studentId, long subjectOfferingId);
    Task<(
        IEnumerable<StudentEnrollment> Items,
        int TotalCount
    )> GetStudentsByOfferingPaginatedAsync(
        long subjectOfferingId,
        int pageNumber,
        int pageSize,
        string? search = null
    );
    Task<bool> IsStudentEnrolledWithProfessorAsync(long studentId, long professorId);
}
