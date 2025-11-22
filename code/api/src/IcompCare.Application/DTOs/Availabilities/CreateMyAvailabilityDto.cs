using System.ComponentModel.DataAnnotations;

namespace IcompCare.Application.DTOs.Availabilities;

public class CreateMyAvailabilityDto
{
    [Required]
    public DateTimeOffset StartTime { get; set; }

    [Required]
    public DateTimeOffset EndTime { get; set; }
}
