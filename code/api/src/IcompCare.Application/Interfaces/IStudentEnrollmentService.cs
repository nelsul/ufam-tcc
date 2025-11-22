using IcompCare.Application.DTOs;
using IcompCare.Application.DTOs.StudentEnrollments;

namespace IcompCare.Application.Interfaces;

public interface IStudentEnrollmentService
{
    Task<IEnumerable<StudentEnrollmentDto>> GetByStudentIdAsync(Guid studentId);
    Task<IEnumerable<StudentEnrollmentDto>> GetBySubjectOfferingIdAsync(Guid subjectOfferingId);
    Task<PagedResult<StudentEnrollmentDto>> GetStudentsByOfferingPaginatedAsync(
        Guid subjectOfferingId,
        int pageNumber,
        int pageSize,
        string? search = null
    );
    Task<bool> IsStudentEnrolledWithProfessorAsync(Guid studentId, Guid professorId);
    Task<StudentEnrollmentDto> CreateAsync(CreateStudentEnrollmentDto dto);
    Task<StudentEnrollmentDto> UpdateAsync(Guid id, UpdateStudentEnrollmentDto dto);
    Task DeleteAsync(Guid id);
}
