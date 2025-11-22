using IcompCare.Domain.Enums;

namespace IcompCare.Domain.Entities;

public class SessionType : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public string Description { get; set; } = string.Empty;
    public GeneralStatus Status { get; set; } = GeneralStatus.Active;

    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
