using IcompCare.Domain.Enums;

namespace IcompCare.Domain.Entities;

public class Availability : BaseEntity
{
    public long ProfessionalId { get; set; }
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset EndTime { get; set; }
    public GeneralStatus Status { get; set; } = GeneralStatus.Active;

    public User Professional { get; set; } = null!;
}
