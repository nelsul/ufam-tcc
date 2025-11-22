using IcompCare.Domain.Enums;

namespace IcompCare.Application.DTOs.Semesters;

public class SemesterDto
{
    public Guid PublicId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public GeneralStatus Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
