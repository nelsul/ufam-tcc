using IcompCare.Domain.Entities;
using IcompCare.Domain.Enums;
using IcompCare.Domain.Interfaces;
using IcompCare.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IcompCare.Infrastructure.Repositories;

public class ObservationRepository : IObservationRepository
{
    private readonly IcompCareDbContext _context;

    public ObservationRepository(IcompCareDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<Observation> Items, int TotalCount)> GetAllAsync(
        int pageNumber,
        int pageSize,
        bool includeInactive = false,
        string? search = null
    )
    {
        var query = _context.Observations.AsQueryable();

        if (!includeInactive)
        {
            query = query.Where(o => o.Status == GeneralStatus.Active);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(o =>
                o.Name.ToLower().Contains(searchLower)
                || (o.Description != null && o.Description.ToLower().Contains(searchLower))
            );
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(o => o.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Observation?> GetByIdAsync(long id)
    {
        return await _context.Observations.FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<Observation?> GetByPublicIdAsync(Guid publicId)
    {
        return await _context.Observations.FirstOrDefaultAsync(o => o.PublicId == publicId);
    }

    public async Task<bool> ExistsByNameAsync(string name, Guid? excludePublicId = null)
    {
        var query = _context.Observations.Where(o =>
            o.Name.ToLower() == name.ToLower() && o.Status == GeneralStatus.Active
        );

        if (excludePublicId.HasValue)
        {
            query = query.Where(o => o.PublicId != excludePublicId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<Observation> AddAsync(Observation observation)
    {
        _context.Observations.Add(observation);
        await _context.SaveChangesAsync();
        return observation;
    }

    public async Task UpdateAsync(Observation observation)
    {
        _context.Entry(observation).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var observation = await _context.Observations.FindAsync(id);
        if (observation != null)
        {
            observation.Status = GeneralStatus.Inactive;
            await _context.SaveChangesAsync();
        }
    }
}
