using System.ComponentModel.DataAnnotations;
using IcompCare.Domain.Enums;

namespace IcompCare.Application.DTOs.Semesters;

public class UpdateSemesterDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public DateOnly StartDate { get; set; }

    [Required]
    public DateOnly EndDate { get; set; }

    public GeneralStatus Status { get; set; }
}
