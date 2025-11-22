using IcompCare.Domain.Enums;

namespace IcompCare.Domain.Entities;

public class Semester : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public GeneralStatus Status { get; set; } = GeneralStatus.Active;

    public ICollection<SubjectOffering> Offerings { get; set; } = new List<SubjectOffering>();
}
