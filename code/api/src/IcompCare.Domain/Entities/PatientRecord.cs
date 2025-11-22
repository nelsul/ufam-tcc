using IcompCare.Domain.Enums;

namespace IcompCare.Domain.Entities;

public class PatientRecord : BaseEntity
{
    public long StudentId { get; set; }
    public string Content { get; set; } = string.Empty;
    public GeneralStatus Status { get; set; } = GeneralStatus.Active;

    public User Student { get; set; } = null!;
}
