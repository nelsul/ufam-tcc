using IcompCare.Application.Interfaces;
using IcompCare.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace IcompCare.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ISubjectService, SubjectService>();
        services.AddScoped<ISemesterService, SemesterService>();
        services.AddScoped<ISubjectOfferingService, SubjectOfferingService>();
        services.AddScoped<IAvailabilityService, AvailabilityService>();
        services.AddScoped<ISessionTypeService, SessionTypeService>();
        services.AddScoped<IAppointmentService, AppointmentService>();
        services.AddScoped<ISessionService, SessionService>();
        services.AddScoped<IPatientRecordService, PatientRecordService>();
        services.AddScoped<IObservationService, ObservationService>();
        services.AddScoped<IPatientObservationService, PatientObservationService>();
        services.AddScoped<IStudentEnrollmentService, StudentEnrollmentService>();

        return services;
    }
}
