namespace IcompCare.Application.DTOs.Observations;

public class ObservationDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class CreateObservationDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateObservationDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; }
}
