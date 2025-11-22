using System.ComponentModel.DataAnnotations;

namespace IcompCare.Application.DTOs.Users;

public class ResetPasswordDto
{
    [Required]
    [MinLength(6)]
    public string NewPassword { get; set; } = string.Empty;
}
