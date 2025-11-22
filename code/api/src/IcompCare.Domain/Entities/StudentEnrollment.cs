using IcompCare.Domain.Enums;

namespace IcompCare.Domain.Entities;

public class StudentEnrollment : BaseEntity
{
    public long StudentId { get; set; }
    public long SubjectOfferingId { get; set; }
    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Enrolled;

    public User Student { get; set; } = null!;
    public SubjectOffering SubjectOffering { get; set; } = null!;
}
