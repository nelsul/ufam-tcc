using IcompCare.Domain.Enums;

namespace IcompCare.Domain.Entities;

public class User : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string InstitutionalEmail { get; set; } = string.Empty;
    public string Registration { get; set; } = string.Empty;
    public string? PasswordHash { get; set; }
    public UserStatus Status { get; set; } = UserStatus.Active;
    public UserRole Role { get; set; } = UserRole.Student;

    public ICollection<SubjectOffering> ProfessorOfferings { get; set; } =
        new List<SubjectOffering>();
    public ICollection<Availability> ProfessionalAvailabilities { get; set; } =
        new List<Availability>();
    public ICollection<Appointment> ProfessionalAppointments { get; set; } =
        new List<Appointment>();
    public ICollection<Session> ProfessionalSessions { get; set; } = new List<Session>();
    public ICollection<PatientRecord> StudentRecords { get; set; } = new List<PatientRecord>();
    public ICollection<PatientObservation> StudentObservations { get; set; } =
        new List<PatientObservation>();
    public ICollection<PatientObservation> ProfessionalObservations { get; set; } =
        new List<PatientObservation>();
}
