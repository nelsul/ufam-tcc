using IcompCare.Domain.Entities;
using IcompCare.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace IcompCare.Infrastructure.Data;

public class IcompCareDbContext(DbContextOptions<IcompCareDbContext> options) : DbContext(options)
{
    public required DbSet<User> Users { get; set; }
    public required DbSet<Semester> Semesters { get; set; }
    public required DbSet<Subject> Subjects { get; set; }
    public required DbSet<SubjectOffering> SubjectOfferings { get; set; }
    public required DbSet<Availability> Availabilities { get; set; }
    public required DbSet<SessionType> SessionTypes { get; set; }
    public required DbSet<Appointment> Appointments { get; set; }
    public required DbSet<Session> Sessions { get; set; }
    public required DbSet<PatientRecord> PatientRecords { get; set; }
    public required DbSet<Observation> Observations { get; set; }
    public required DbSet<PatientObservation> PatientObservations { get; set; }
    public required DbSet<StudentEnrollment> StudentEnrollments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasPostgresEnum<GeneralStatus>("general_status_enum");
        modelBuilder.HasPostgresEnum<UserStatus>("user_status_enum");
        modelBuilder.HasPostgresEnum<UserRole>("user_role_enum");
        modelBuilder.HasPostgresEnum<AppointmentStatus>("appointment_status_enum");
        modelBuilder.HasPostgresEnum<SessionStatus>("session_status_enum");
        modelBuilder.HasPostgresEnum<EnrollmentStatus>("enrollment_status_enum");


        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.InstitutionalEmail).IsUnique();
            entity.HasIndex(e => e.Registration).IsUnique();
            entity.HasIndex(e => e.PublicId).IsUnique();

            entity.Property(e => e.Status).HasColumnType("user_status_enum");
            entity.Property(e => e.Role).HasColumnType("user_role_enum");
        });

        modelBuilder.Entity<Semester>(entity =>
        {
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.Status).HasColumnType("general_status_enum");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasIndex(e => e.Code).IsUnique();
            entity.Property(e => e.Status).HasColumnType("general_status_enum");
        });

        modelBuilder.Entity<SubjectOffering>(entity =>
        {
            entity
                .HasIndex(e => new
                {
                    e.SemesterId,
                    e.SubjectId,
                    e.ProfessorId,
                })
                .IsUnique();
            entity.Property(e => e.Status).HasColumnType("general_status_enum");
        });

        modelBuilder.Entity<Availability>(entity =>
        {
            entity.Property(e => e.Status).HasColumnType("general_status_enum");
        });

        modelBuilder.Entity<SessionType>(entity =>
        {
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.Status).HasColumnType("general_status_enum");
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.Property(e => e.Status).HasColumnType("appointment_status_enum");

            entity
                .HasOne(d => d.Professional)
                .WithMany(p => p.ProfessionalAppointments)
                .HasForeignKey(d => d.ProfessionalId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(d => d.Student)
                .WithMany()
                .HasForeignKey(d => d.StudentId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Session>(entity =>
        {
            entity.Property(e => e.Status).HasColumnType("session_status_enum");

            entity
                .HasOne(d => d.Appointment)
                .WithOne(p => p.Session)
                .HasForeignKey<Session>(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(d => d.Professional)
                .WithMany(p => p.ProfessionalSessions)
                .HasForeignKey(d => d.ProfessionalId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(d => d.Student)
                .WithMany()
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PatientRecord>(entity =>
        {
            entity.Property(e => e.Status).HasColumnType("general_status_enum");
        });

        modelBuilder.Entity<Observation>(entity =>
        {
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.Status).HasColumnType("general_status_enum");
        });

        modelBuilder.Entity<PatientObservation>(entity =>
        {
            entity.Property(e => e.Status).HasColumnType("general_status_enum");

            entity
                .HasOne(d => d.Student)
                .WithMany(p => p.StudentObservations)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(d => d.Professional)
                .WithMany(p => p.ProfessionalObservations)
                .HasForeignKey(d => d.ProfessionalId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<StudentEnrollment>(entity =>
        {
            entity.HasIndex(e => e.PublicId).IsUnique();
            entity.HasIndex(e => e.StudentId);
            entity.HasIndex(e => e.SubjectOfferingId);
            entity.HasIndex(e => new { e.StudentId, e.SubjectOfferingId }).IsUnique();

            entity.Property(e => e.Status).HasColumnType("enrollment_status_enum");

            entity
                .HasOne(d => d.Student)
                .WithMany()
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasOne(d => d.SubjectOffering)
                .WithMany()
                .HasForeignKey(d => d.SubjectOfferingId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
