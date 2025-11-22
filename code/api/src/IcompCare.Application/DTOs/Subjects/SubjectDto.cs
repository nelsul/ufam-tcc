using IcompCare.Domain.Enums;

namespace IcompCare.Application.DTOs.Subjects;

public class SubjectDto
{
    public Guid PublicId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public GeneralStatus Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
