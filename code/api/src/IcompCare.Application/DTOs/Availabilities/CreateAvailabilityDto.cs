using System.ComponentModel.DataAnnotations;

namespace IcompCare.Application.DTOs.Availabilities;

public class CreateAvailabilityDto
{
    [Required]
    public Guid ProfessionalId { get; set; }

    [Required]
    public DateTimeOffset StartTime { get; set; }

    [Required]
    public DateTimeOffset EndTime { get; set; }
}
