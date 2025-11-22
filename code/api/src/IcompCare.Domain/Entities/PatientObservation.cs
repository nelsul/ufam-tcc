using IcompCare.Domain.Enums;

namespace IcompCare.Domain.Entities;

public class PatientObservation : BaseEntity
{
    public long StudentId { get; set; }
    public long ObservationId { get; set; }
    public long ProfessionalId { get; set; }
    public string Notes { get; set; } = string.Empty;
    public GeneralStatus Status { get; set; } = GeneralStatus.Active;

    public User Student { get; set; } = null!;
    public Observation Observation { get; set; } = null!;
    public User Professional { get; set; } = null!;
}
