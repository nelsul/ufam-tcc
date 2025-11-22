using IcompCare.Application.DTOs.Users;
using IcompCare.Domain.Enums;

namespace IcompCare.Application.DTOs.Availabilities;

public class AvailabilityDto
{
    public Guid PublicId { get; set; }
    public UserDto Professional { get; set; } = null!;
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset EndTime { get; set; }
    public GeneralStatus Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
