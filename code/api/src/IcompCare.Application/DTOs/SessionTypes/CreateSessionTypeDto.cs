using System.ComponentModel.DataAnnotations;

namespace IcompCare.Application.DTOs.SessionTypes;

public class CreateSessionTypeDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Duration must be greater than 0")]
    public int DurationMinutes { get; set; }

    [Required]
    public string Description { get; set; } = string.Empty;
}
