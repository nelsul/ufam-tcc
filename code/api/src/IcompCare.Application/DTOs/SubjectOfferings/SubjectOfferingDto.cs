using IcompCare.Application.DTOs.Semesters;
using IcompCare.Application.DTOs.Subjects;
using IcompCare.Application.DTOs.Users;
using IcompCare.Domain.Enums;

namespace IcompCare.Application.DTOs.SubjectOfferings;

public class SubjectOfferingDto
{
    public Guid PublicId { get; set; }
    public SemesterDto Semester { get; set; } = null!;
    public SubjectDto Subject { get; set; } = null!;
    public UserDto Professor { get; set; } = null!;
    public GeneralStatus Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
