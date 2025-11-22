using IcompCare.Domain.Entities;

namespace IcompCare.Domain.Interfaces;

public interface ISessionRepository
{
    Task<(IEnumerable<Session> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize);
    Task<(IEnumerable<Session> Items, int TotalCount)> GetByProfessionalIdAsync(
        long professionalId,
        int pageNumber,
        int pageSize,
        DateTimeOffset? dateFrom = null,
        DateTimeOffset? dateTo = null,
        string? search = null
    );
    Task<Session?> GetByIdAsync(long id);
    Task<Session?> GetByPublicIdAsync(Guid publicId);
    Task<Session?> GetByAppointmentIdAsync(long appointmentId);
    Task<Session?> GetOpenSessionByProfessionalIdAsync(long professionalId);
    Task<Session> AddAsync(Session session);
    Task UpdateAsync(Session session);
    Task DeleteAsync(long id);
}
