using IcompCare.Application.DTOs.SubjectOfferings;
using IcompCare.Application.DTOs.Users;
using IcompCare.Domain.Enums;

namespace IcompCare.Application.DTOs.StudentEnrollments;

public class StudentEnrollmentDto
{
    public Guid PublicId { get; set; }
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentRegistration { get; set; } = string.Empty;
    public string StudentEmail { get; set; } = string.Empty;
    public Guid SubjectOfferingId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public string SubjectCode { get; set; } = string.Empty;
    public string SemesterName { get; set; } = string.Empty;
    public EnrollmentStatus Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class CreateStudentEnrollmentDto
{
    public Guid StudentId { get; set; }
    public Guid SubjectOfferingId { get; set; }
    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Enrolled;
}

public class UpdateStudentEnrollmentDto
{
    public EnrollmentStatus Status { get; set; }
}
