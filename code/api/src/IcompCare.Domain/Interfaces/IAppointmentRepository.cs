using IcompCare.Domain.Entities;

namespace IcompCare.Domain.Interfaces;

public interface IAppointmentRepository
{
    Task<(IEnumerable<Appointment> Items, int TotalCount)> GetAllAsync(
        int pageNumber,
        int pageSize
    );
    Task<Appointment?> GetByIdAsync(long id);
    Task<Appointment?> GetByPublicIdAsync(Guid publicId);
    Task<bool> HasOverlapAsync(
        long professionalId,
        DateTimeOffset startTime,
        DateTimeOffset endTime,
        Guid? excludePublicId = null
    );
    Task<Appointment> AddAsync(Appointment appointment);
    Task UpdateAsync(Appointment appointment);
    Task DeleteAsync(long id);
    Task<(IEnumerable<Appointment> Items, int TotalCount)> GetByProfessionalIdAsync(
        long professionalId,
        int pageNumber = 1,
        int pageSize = 20,
        string? status = null,
        Guid? sessionTypeId = null,
        string? search = null
    );
    Task<IEnumerable<Appointment>> GetAllByProfessionalIdAsync(long professionalId);
}
