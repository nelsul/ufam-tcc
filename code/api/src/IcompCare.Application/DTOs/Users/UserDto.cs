using IcompCare.Domain.Enums;

namespace IcompCare.Application.DTOs.Users;

public class UserDto
{
    public Guid PublicId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string InstitutionalEmail { get; set; } = string.Empty;
    public string Registration { get; set; } = string.Empty;
    public UserStatus Status { get; set; }
    public UserRole Role { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
