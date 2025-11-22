using IcompCare.Domain.Enums;

namespace IcompCare.Domain.Entities;

public class SubjectOffering : BaseEntity
{
    public long SemesterId { get; set; }
    public long SubjectId { get; set; }
    public long ProfessorId { get; set; }
    public GeneralStatus Status { get; set; } = GeneralStatus.Active;

    public Semester Semester { get; set; } = null!;
    public Subject Subject { get; set; } = null!;
    public User Professor { get; set; } = null!;
}
