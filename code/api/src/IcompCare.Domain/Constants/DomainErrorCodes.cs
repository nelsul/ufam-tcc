namespace IcompCare.Domain.Constants;

public static class DomainErrorCodes
{
    public static class User
    {
        public const string EmailAlreadyRegistered = "User.EmailAlreadyRegistered";
        public const string RegistrationAlreadyExists = "User.RegistrationAlreadyExists";
        public const string NotFound = "User.NotFound";
        public const string InvalidEmail = "User.InvalidEmail";
    }

    public static class Subject
    {
        public const string CodeAlreadyExists = "Subject.CodeAlreadyExists";
        public const string NotFound = "Subject.NotFound";
    }

    public static class Semester
    {
        public const string NameAlreadyExists = "Semester.NameAlreadyExists";
        public const string NotFound = "Semester.NotFound";
        public const string InvalidDateRange = "Semester.InvalidDateRange";
    }

    public static class SubjectOffering
    {
        public const string AlreadyExists = "SubjectOffering.AlreadyExists";
        public const string NotFound = "SubjectOffering.NotFound";
    }

    public static class Availability
    {
        public const string NotFound = "Availability.NotFound";
        public const string InvalidTimeRange = "Availability.InvalidTimeRange";
        public const string Overlap = "Availability.Overlap";
    }

    public static class SessionType
    {
        public const string NameAlreadyExists = "SessionType.NameAlreadyExists";
        public const string NotFound = "SessionType.NotFound";
        public const string InvalidDuration = "SessionType.InvalidDuration";
    }

    public static class Appointment
    {
        public const string NotFound = "Appointment.NotFound";
        public const string InvalidTimeRange = "Appointment.InvalidTimeRange";
        public const string Overlap = "Appointment.Overlap";
        public const string NoAvailability = "Appointment.NoAvailability";
    }

    public static class Session
    {
        public const string NotFound = "Session.NotFound";
        public const string AlreadyExistsForAppointment = "Session.AlreadyExistsForAppointment";
        public const string ProfessionalHasOpenSession = "Session.ProfessionalHasOpenSession";
        public const string AppointmentNotConfirmed = "Session.AppointmentNotConfirmed";
        public const string StudentNotLinked = "Session.StudentNotLinked";
    }

    public static class PatientRecord
    {
        public const string NotFound = "PatientRecord.NotFound";
    }

    public static class Observation
    {
        public const string NotFound = "Observation.NotFound";
        public const string NameAlreadyExists = "Observation.NameAlreadyExists";
    }

    public static class PatientObservation
    {
        public const string NotFound = "PatientObservation.NotFound";
    }

    public const string InternalServerError = "Server.InternalError";
    public const string NotFound = "General.NotFound";
    public const string Conflict = "General.Conflict";
    public const string Validation = "General.Validation";

    public static class Common
    {
        public const string NotFound = "Common.NotFound";
        public const string Validation = "Common.Validation";
        public const string Conflict = "Common.Conflict";
        public const string Forbidden = "Common.Forbidden";
    }
}
