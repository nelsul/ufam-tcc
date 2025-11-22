namespace IcompCare.Application.DTOs.Appointments;

public class AppointmentDto
{
    public Guid Id { get; set; }
    public Guid ProfessionalId { get; set; }
    public string ProfessionalName { get; set; } = string.Empty;
    public Guid? StudentId { get; set; }
    public string StudentEmail { get; set; } = string.Empty;
    public string StudentRegistration { get; set; } = string.Empty;
    public string StudentFullName { get; set; } = string.Empty;
    public Guid? SessionTypeId { get; set; }
    public string? SessionTypeName { get; set; }
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset? EndTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public string ReasonForVisit { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class CreateAppointmentDto
{
    public Guid ProfessionalId { get; set; }
    public string StudentEmail { get; set; } = string.Empty;
    public string StudentRegistration { get; set; } = string.Empty;
    public string StudentFullName { get; set; } = string.Empty;
    public DateTimeOffset StartTime { get; set; }
    public string ReasonForVisit { get; set; } = string.Empty;
}

public class UpdateAppointmentDto
{
    public Guid? StudentId { get; set; }
    public Guid? SessionTypeId { get; set; }
    public DateTimeOffset? StartTime { get; set; }
    public DateTimeOffset? EndTime { get; set; }
    public string? ReasonForVisit { get; set; }
    public string? Status { get; set; }
}
