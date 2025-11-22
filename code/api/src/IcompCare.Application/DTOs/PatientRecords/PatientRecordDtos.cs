namespace IcompCare.Application.DTOs.PatientRecords;

public class PatientRecordDto
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class CreatePatientRecordDto
{
    public Guid StudentId { get; set; }
    public string Content { get; set; } = string.Empty;
}

public class UpdatePatientRecordDto
{
    public string Content { get; set; } = string.Empty;
}
