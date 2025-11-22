using IcompCare.Domain.Enums;
using IcompCare.Domain.Interfaces;
using IcompCare.Infrastructure.Data;
using IcompCare.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace IcompCare.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);

        dataSourceBuilder.MapEnum<UserRole>("user_role_enum");
        dataSourceBuilder.MapEnum<UserStatus>("user_status_enum");
        dataSourceBuilder.MapEnum<GeneralStatus>("general_status_enum");
        dataSourceBuilder.MapEnum<AppointmentStatus>("appointment_status_enum");
        dataSourceBuilder.MapEnum<SessionStatus>("session_status_enum");
        dataSourceBuilder.MapEnum<EnrollmentStatus>("enrollment_status_enum");
        dataSourceBuilder.EnableDynamicJson();

        var dataSource = dataSourceBuilder.Build();

        services.AddDbContext<IcompCareDbContext>(options =>
            options.UseNpgsql(dataSource).UseSnakeCaseNamingConvention()
        );

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ISubjectRepository, SubjectRepository>();
        services.AddScoped<ISemesterRepository, SemesterRepository>();
        services.AddScoped<ISubjectOfferingRepository, SubjectOfferingRepository>();
        services.AddScoped<IAvailabilityRepository, AvailabilityRepository>();
        services.AddScoped<ISessionTypeRepository, SessionTypeRepository>();
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<ISessionRepository, SessionRepository>();
        services.AddScoped<IPatientRecordRepository, PatientRecordRepository>();
        services.AddScoped<IObservationRepository, ObservationRepository>();
        services.AddScoped<IPatientObservationRepository, PatientObservationRepository>();
        services.AddScoped<IStudentEnrollmentRepository, StudentEnrollmentRepository>();

        return services;
    }
}
