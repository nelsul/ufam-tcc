namespace IcompCare.Application.DTOs.Sessions;

public class SessionDto
{
    public Guid Id { get; set; }
    public Guid AppointmentId { get; set; }
    public Guid ProfessionalId { get; set; }
    public string ProfessionalName { get; set; } = string.Empty;
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset? EndedAt { get; set; }
    public string Notes { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class CreateSessionDto
{
    public Guid AppointmentId { get; set; }
    public Guid ProfessionalId { get; set; }
    public Guid StudentId { get; set; }
    public DateTimeOffset StartedAt { get; set; }
    public string Notes { get; set; } = string.Empty;
}

public class UpdateSessionDto
{
    public DateTimeOffset? EndedAt { get; set; }
    public string? Notes { get; set; }
    public string? Status { get; set; }
}
