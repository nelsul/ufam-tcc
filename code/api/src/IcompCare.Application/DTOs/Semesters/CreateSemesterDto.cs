using System.ComponentModel.DataAnnotations;

namespace IcompCare.Application.DTOs.Semesters;

public class CreateSemesterDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public DateOnly StartDate { get; set; }

    [Required]
    public DateOnly EndDate { get; set; }
}
