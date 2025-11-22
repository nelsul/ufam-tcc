using System.ComponentModel.DataAnnotations;
using IcompCare.Domain.Enums;

namespace IcompCare.Application.DTOs.Users;

public class UpdateUserDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string InstitutionalEmail { get; set; } = string.Empty;

    [Required]
    public string Registration { get; set; } = string.Empty;

    public UserStatus Status { get; set; }

    public UserRole Role { get; set; }
}
