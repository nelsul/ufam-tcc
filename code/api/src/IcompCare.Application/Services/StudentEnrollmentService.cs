using IcompCare.Application.DTOs;
using IcompCare.Application.DTOs.StudentEnrollments;
using IcompCare.Application.Interfaces;
using IcompCare.Domain.Entities;
using IcompCare.Domain.Interfaces;

namespace IcompCare.Application.Services;

public class StudentEnrollmentService : IStudentEnrollmentService
{
    private readonly IStudentEnrollmentRepository _repository;
    private readonly IUserRepository _userRepository;
    private readonly ISubjectOfferingRepository _subjectOfferingRepository;

    public StudentEnrollmentService(
        IStudentEnrollmentRepository repository,
        IUserRepository userRepository,
        ISubjectOfferingRepository subjectOfferingRepository
    )
    {
        _repository = repository;
        _userRepository = userRepository;
        _subjectOfferingRepository = subjectOfferingRepository;
    }

    public async Task<IEnumerable<StudentEnrollmentDto>> GetByStudentIdAsync(Guid studentId)
    {
        var student = await _userRepository.GetByPublicIdAsync(studentId);
        if (student == null)
            throw new KeyNotFoundException("Student not found");

        var enrollments = await _repository.GetByStudentIdAsync(student.Id);
        return enrollments.Select(MapToDto);
    }

    public async Task<IEnumerable<StudentEnrollmentDto>> GetBySubjectOfferingIdAsync(
        Guid subjectOfferingId
    )
    {
        var offering = await _subjectOfferingRepository.GetByPublicIdAsync(subjectOfferingId);
        if (offering == null)
            throw new KeyNotFoundException("Subject Offering not found");

        var enrollments = await _repository.GetBySubjectOfferingIdAsync(offering.Id);
        return enrollments.Select(MapToDto);
    }

    public async Task<PagedResult<StudentEnrollmentDto>> GetStudentsByOfferingPaginatedAsync(
        Guid subjectOfferingId,
        int pageNumber,
        int pageSize,
        string? search = null
    )
    {
        var offering = await _subjectOfferingRepository.GetByPublicIdAsync(subjectOfferingId);
        if (offering == null)
            throw new KeyNotFoundException("Subject Offering not found");

        var (items, totalCount) = await _repository.GetStudentsByOfferingPaginatedAsync(
            offering.Id,
            pageNumber,
            pageSize,
            search
        );

        return new PagedResult<StudentEnrollmentDto>
        {
            Items = items.Select(MapToDto),
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
        };
    }

    public async Task<StudentEnrollmentDto> CreateAsync(CreateStudentEnrollmentDto dto)
    {
        var student = await _userRepository.GetByPublicIdAsync(dto.StudentId);
        if (student == null)
            throw new KeyNotFoundException("Student not found");

        var offering = await _subjectOfferingRepository.GetByPublicIdAsync(dto.SubjectOfferingId);
        if (offering == null)
            throw new KeyNotFoundException("Subject Offering not found");

        var existing = await _repository.GetByStudentAndOfferingAsync(student.Id, offering.Id);
        if (existing != null)
            throw new InvalidOperationException("Student already enrolled in this offering");

        var enrollment = new StudentEnrollment
        {
            StudentId = student.Id,
            SubjectOfferingId = offering.Id,
            Status = dto.Status,
        };

        var created = await _repository.AddAsync(enrollment);

        created = await _repository.GetByIdAsync(created.Id) ?? created;

        return MapToDto(created);
    }

    public async Task<StudentEnrollmentDto> UpdateAsync(Guid id, UpdateStudentEnrollmentDto dto)
    {
        var enrollment = await _repository.GetByPublicIdAsync(id);
        if (enrollment == null)
            throw new KeyNotFoundException("Enrollment not found");

        enrollment.Status = dto.Status;
        await _repository.UpdateAsync(enrollment);

        return MapToDto(enrollment);
    }

    public async Task DeleteAsync(Guid id)
    {
        var enrollment = await _repository.GetByPublicIdAsync(id);
        if (enrollment == null)
            throw new KeyNotFoundException("Enrollment not found");

        await _repository.DeleteAsync(enrollment);
    }

    public async Task<bool> IsStudentEnrolledWithProfessorAsync(Guid studentId, Guid professorId)
    {
        var student = await _userRepository.GetByPublicIdAsync(studentId);
        if (student == null)
            return false;

        var professor = await _userRepository.GetByPublicIdAsync(professorId);
        if (professor == null)
            return false;

        return await _repository.IsStudentEnrolledWithProfessorAsync(student.Id, professor.Id);
    }

    private static StudentEnrollmentDto MapToDto(StudentEnrollment entity)
    {
        return new StudentEnrollmentDto
        {
            PublicId = entity.PublicId,
            StudentId = entity.Student.PublicId,
            StudentName = entity.Student.Name,
            StudentRegistration = entity.Student.Registration ?? string.Empty,
            StudentEmail = entity.Student.InstitutionalEmail,
            SubjectOfferingId = entity.SubjectOffering.PublicId,
            SubjectName = entity.SubjectOffering.Subject.Name,
            SubjectCode = entity.SubjectOffering.Subject.Code,
            SemesterName = entity.SubjectOffering.Semester.Name,
            Status = entity.Status,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
        };
    }
}
