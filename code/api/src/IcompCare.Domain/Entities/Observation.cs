using IcompCare.Domain.Enums;

namespace IcompCare.Domain.Entities;

public class Observation : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public GeneralStatus Status { get; set; } = GeneralStatus.Active;

    public ICollection<PatientObservation> PatientObservations { get; set; } =
        new List<PatientObservation>();
}
