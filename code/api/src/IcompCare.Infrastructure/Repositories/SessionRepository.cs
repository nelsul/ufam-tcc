using IcompCare.Domain.Entities;
using IcompCare.Domain.Enums;
using IcompCare.Domain.Interfaces;
using IcompCare.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IcompCare.Infrastructure.Repositories;

public class SessionRepository : ISessionRepository
{
    private readonly IcompCareDbContext _context;

    public SessionRepository(IcompCareDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<Session> Items, int TotalCount)> GetAllAsync(
        int pageNumber,
        int pageSize
    )
    {
        var query = _context
            .Sessions.Include(s => s.Appointment)
            .Include(s => s.Professional)
            .Include(s => s.Student)
            .AsQueryable();

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(s => s.StartedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<(IEnumerable<Session> Items, int TotalCount)> GetByProfessionalIdAsync(
        long professionalId,
        int pageNumber,
        int pageSize,
        DateTimeOffset? dateFrom = null,
        DateTimeOffset? dateTo = null,
        string? search = null
    )
    {
        var query = _context
            .Sessions.Include(s => s.Appointment)
            .Include(s => s.Professional)
            .Include(s => s.Student)
            .Where(s => s.ProfessionalId == professionalId);

        if (dateFrom.HasValue)
        {
            query = query.Where(s => s.StartedAt >= dateFrom.Value);
        }

        if (dateTo.HasValue)
        {
            query = query.Where(s => s.StartedAt <= dateTo.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(s =>
                (s.Student != null && s.Student.Name.ToLower().Contains(searchLower))
                || (s.Student != null && s.Student.FullName.ToLower().Contains(searchLower))
                || (
                    s.Student != null
                    && s.Student.InstitutionalEmail.ToLower().Contains(searchLower)
                )
                || (s.Student != null && s.Student.Registration.ToLower().Contains(searchLower))
            );
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(s => s.StartedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Session?> GetByIdAsync(long id)
    {
        return await _context
            .Sessions.Include(s => s.Appointment)
            .Include(s => s.Professional)
            .Include(s => s.Student)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Session?> GetByPublicIdAsync(Guid publicId)
    {
        return await _context
            .Sessions.Include(s => s.Appointment)
            .Include(s => s.Professional)
            .Include(s => s.Student)
            .FirstOrDefaultAsync(s => s.PublicId == publicId);
    }

    public async Task<Session?> GetByAppointmentIdAsync(long appointmentId)
    {
        return await _context
            .Sessions.Include(s => s.Appointment)
            .Include(s => s.Professional)
            .Include(s => s.Student)
            .FirstOrDefaultAsync(s => s.AppointmentId == appointmentId);
    }

    public async Task<Session?> GetOpenSessionByProfessionalIdAsync(long professionalId)
    {
        return await _context
            .Sessions.Include(s => s.Appointment)
            .Include(s => s.Professional)
            .Include(s => s.Student)
            .FirstOrDefaultAsync(s =>
                s.ProfessionalId == professionalId && s.Status == SessionStatus.InProgress
            );
    }

    public async Task<Session> AddAsync(Session session)
    {
        _context.Sessions.Add(session);
        await _context.SaveChangesAsync();
        return session;
    }

    public async Task UpdateAsync(Session session)
    {
        _context.Entry(session).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var session = await _context.Sessions.FindAsync(id);
        if (session != null)
        {
            session.Status = SessionStatus.Cancelled;
            await _context.SaveChangesAsync();
        }
    }
}
