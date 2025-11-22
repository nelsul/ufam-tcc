using IcompCare.Domain.Enums;

namespace IcompCare.Application.DTOs.SessionTypes;

public class SessionTypeDto
{
    public Guid PublicId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public string Description { get; set; } = string.Empty;
    public GeneralStatus Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
