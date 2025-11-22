using System.ComponentModel.DataAnnotations;

namespace IcompCare.Application.DTOs.SubjectOfferings;

public class CreateSubjectOfferingDto
{
    [Required]
    public Guid SemesterId { get; set; }

    [Required]
    public Guid SubjectId { get; set; }

    [Required]
    public Guid ProfessorId { get; set; }
}
