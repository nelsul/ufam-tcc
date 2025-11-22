using System.ComponentModel.DataAnnotations;
using IcompCare.Domain.Enums;

namespace IcompCare.Application.DTOs.Subjects;

public class UpdateSubjectDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Code { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    public GeneralStatus Status { get; set; }
}
