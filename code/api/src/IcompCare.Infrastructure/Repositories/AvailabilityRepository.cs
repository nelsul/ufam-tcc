using IcompCare.Domain.Entities;
using IcompCare.Domain.Enums;
using IcompCare.Domain.Interfaces;
using IcompCare.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IcompCare.Infrastructure.Repositories;

public class AvailabilityRepository : IAvailabilityRepository
{
    private readonly IcompCareDbContext _context;

    public AvailabilityRepository(IcompCareDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Availability>> GetAllAsync(bool includeInactive = false)
    {
        var query = _context.Availabilities.Include(a => a.Professional).AsQueryable();
        if (!includeInactive)
        {
            query = query.Where(a => a.Status == GeneralStatus.Active);
        }
        return await query.ToListAsync();
    }

    public async Task<(IEnumerable<Availability> Items, int TotalCount)> GetAllAsync(
        int pageNumber,
        int pageSize,
        bool includeInactive = false
    )
    {
        var query = _context.Availabilities.Include(a => a.Professional).AsQueryable();
        if (!includeInactive)
        {
            query = query.Where(a => a.Status == GeneralStatus.Active);
        }

        var totalCount = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return (items, totalCount);
    }

    public async Task<Availability?> GetByIdAsync(long id)
    {
        return await _context
            .Availabilities.Include(a => a.Professional)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Availability?> GetByPublicIdAsync(Guid publicId)
    {
        return await _context
            .Availabilities.Include(a => a.Professional)
            .FirstOrDefaultAsync(a => a.PublicId == publicId);
    }

    public async Task<IEnumerable<Availability>> GetByProfessionalIdAsync(long professionalId)
    {
        return await _context
            .Availabilities.Include(a => a.Professional)
            .Where(a => a.ProfessionalId == professionalId && a.Status == GeneralStatus.Active)
            .ToListAsync();
    }

    public async Task<bool> HasOverlapAsync(
        long professionalId,
        DateTimeOffset startTime,
        DateTimeOffset endTime,
        Guid? excludePublicId = null
    )
    {
        var query = _context.Availabilities.Where(a =>
            a.ProfessionalId == professionalId && a.Status == GeneralStatus.Active
        );

        if (excludePublicId.HasValue)
        {
            query = query.Where(a => a.PublicId != excludePublicId.Value);
        }

        return await query.AnyAsync(a => a.StartTime < endTime && a.EndTime > startTime);
    }

    public async Task<Availability> AddAsync(Availability availability)
    {
        _context.Availabilities.Add(availability);
        await _context.SaveChangesAsync();
        return availability;
    }

    public async Task UpdateAsync(Availability availability)
    {
        _context.Entry(availability).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var availability = await _context.Availabilities.FindAsync(id);
        if (availability != null)
        {
            availability.Status = GeneralStatus.Inactive;
            await _context.SaveChangesAsync();
        }
    }
}
