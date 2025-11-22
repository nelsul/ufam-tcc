using IcompCare.Domain.Enums;

namespace IcompCare.Domain.Entities;

public class Appointment : BaseEntity
{
    public long ProfessionalId { get; set; }
    public long? StudentId { get; set; }
    public string StudentEmail { get; set; } = string.Empty;
    public string StudentRegistration { get; set; } = string.Empty;
    public string StudentFullName { get; set; } = string.Empty;
    public long? SessionTypeId { get; set; }
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset? EndTime { get; set; }
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
    public string ReasonForVisit { get; set; } = string.Empty;

    public User Professional { get; set; } = null!;
    public User? Student { get; set; }
    public SessionType? SessionType { get; set; }
    public Session? Session { get; set; }
}
