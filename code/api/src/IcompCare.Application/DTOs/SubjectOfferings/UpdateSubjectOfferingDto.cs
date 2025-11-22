using IcompCare.Domain.Enums;

namespace IcompCare.Application.DTOs.SubjectOfferings;

public class UpdateSubjectOfferingDto
{
    public Guid SemesterId { get; set; }
    public Guid SubjectId { get; set; }
    public Guid ProfessorId { get; set; }
    public GeneralStatus Status { get; set; }
}
