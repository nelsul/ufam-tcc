using IcompCare.Domain.Entities;
using IcompCare.Domain.Enums;
using IcompCare.Domain.Interfaces;
using IcompCare.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IcompCare.Infrastructure.Repositories;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly IcompCareDbContext _context;

    public AppointmentRepository(IcompCareDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<Appointment> Items, int TotalCount)> GetAllAsync(
        int pageNumber,
        int pageSize
    )
    {
        var query = _context
            .Appointments.Include(a => a.Professional)
            .Include(a => a.Student)
            .Include(a => a.SessionType)
            .AsQueryable();

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(a => a.StartTime)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Appointment?> GetByIdAsync(long id)
    {
        return await _context
            .Appointments.Include(a => a.Professional)
            .Include(a => a.Student)
            .Include(a => a.SessionType)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Appointment?> GetByPublicIdAsync(Guid publicId)
    {
        return await _context
            .Appointments.Include(a => a.Professional)
            .Include(a => a.Student)
            .Include(a => a.SessionType)
            .FirstOrDefaultAsync(a => a.PublicId == publicId);
    }

    public async Task<bool> HasOverlapAsync(
        long professionalId,
        DateTimeOffset startTime,
        DateTimeOffset endTime,
        Guid? excludePublicId = null
    )
    {
        var query = _context.Appointments.Where(a =>
            a.ProfessionalId == professionalId
            && a.Status != AppointmentStatus.Cancelled
            && a.StartTime < endTime
            && a.EndTime > startTime
        );

        if (excludePublicId.HasValue)
        {
            query = query.Where(a => a.PublicId != excludePublicId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<Appointment> AddAsync(Appointment appointment)
    {
        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();
        return appointment;
    }

    public async Task UpdateAsync(Appointment appointment)
    {
        _context.Entry(appointment).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment != null)
        {
            appointment.Status = AppointmentStatus.Cancelled;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<(IEnumerable<Appointment> Items, int TotalCount)> GetByProfessionalIdAsync(
        long professionalId,
        int pageNumber = 1,
        int pageSize = 20,
        string? status = null,
        Guid? sessionTypeId = null,
        string? search = null
    )
    {
        var query = _context
            .Appointments.Include(a => a.Student)
            .Include(a => a.SessionType)
            .Where(a => a.ProfessionalId == professionalId);

        if (!string.IsNullOrWhiteSpace(status))
        {
            if (Enum.TryParse<AppointmentStatus>(status, true, out var statusEnum))
            {
                query = query.Where(a => a.Status == statusEnum);
            }
        }

        if (sessionTypeId.HasValue)
        {
            query = query.Where(a =>
                a.SessionType != null && a.SessionType.PublicId == sessionTypeId.Value
            );
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(a =>
                (a.StudentFullName != null && a.StudentFullName.ToLower().Contains(searchLower))
                || (a.StudentEmail != null && a.StudentEmail.ToLower().Contains(searchLower))
                || (
                    a.StudentRegistration != null
                    && a.StudentRegistration.ToLower().Contains(searchLower)
                )
            );
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(a => a.StartTime)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<IEnumerable<Appointment>> GetAllByProfessionalIdAsync(long professionalId)
    {
        return await _context
            .Appointments.Include(a => a.Student)
            .Include(a => a.SessionType)
            .Where(a => a.ProfessionalId == professionalId)
            .OrderBy(a => a.StartTime)
            .ToListAsync();
    }
}
