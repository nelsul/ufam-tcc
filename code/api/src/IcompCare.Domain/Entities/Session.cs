using IcompCare.Domain.Enums;

namespace IcompCare.Domain.Entities;

public class Session : BaseEntity
{
    public long AppointmentId { get; set; }
    public long ProfessionalId { get; set; }
    public long StudentId { get; set; }
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset? EndedAt { get; set; }
    public string Notes { get; set; } = string.Empty;
    public SessionStatus Status { get; set; } = SessionStatus.InProgress;

    public Appointment Appointment { get; set; } = null!;
    public User Professional { get; set; } = null!;
    public User Student { get; set; } = null!;
}
