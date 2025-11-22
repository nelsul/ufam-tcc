namespace IcompCare.Application.DTOs.PatientObservations;

public class PatientObservationDto
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public Guid ObservationId { get; set; }
    public string ObservationName { get; set; } = string.Empty;
    public Guid ProfessionalId { get; set; }
    public string ProfessionalName { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class CreatePatientObservationDto
{
    public Guid StudentId { get; set; }
    public Guid ObservationId { get; set; }
    public Guid ProfessionalId { get; set; }
    public string Notes { get; set; } = string.Empty;
}

public class UpdatePatientObservationDto
{
    public string Notes { get; set; } = string.Empty;
}
