using IcompCare.Domain.Entities;
using IcompCare.Domain.Enums;
using IcompCare.Domain.Interfaces;
using IcompCare.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IcompCare.Infrastructure.Repositories;

public class SessionTypeRepository : ISessionTypeRepository
{
    private readonly IcompCareDbContext _context;

    public SessionTypeRepository(IcompCareDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SessionType>> GetAllAsync(bool includeInactive = false)
    {
        var query = _context.SessionTypes.AsQueryable();
        if (!includeInactive)
        {
            query = query.Where(st => st.Status == GeneralStatus.Active);
        }
        return await query.ToListAsync();
    }

    public async Task<(IEnumerable<SessionType> Items, int TotalCount)> GetAllAsync(
        int pageNumber,
        int pageSize,
        bool includeInactive = false,
        string? search = null
    )
    {
        var query = _context.SessionTypes.AsQueryable();
        if (!includeInactive)
        {
            query = query.Where(st => st.Status == GeneralStatus.Active);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(st => st.Name.ToLower().Contains(searchLower));
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(st => st.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<SessionType?> GetByIdAsync(long id)
    {
        return await _context.SessionTypes.FindAsync(id);
    }

    public async Task<SessionType?> GetByPublicIdAsync(Guid publicId)
    {
        return await _context.SessionTypes.FirstOrDefaultAsync(st => st.PublicId == publicId);
    }

    public async Task<SessionType?> GetByNameAsync(string name)
    {
        return await _context.SessionTypes.FirstOrDefaultAsync(st => st.Name == name);
    }

    public async Task<SessionType> AddAsync(SessionType sessionType)
    {
        _context.SessionTypes.Add(sessionType);
        await _context.SaveChangesAsync();
        return sessionType;
    }

    public async Task UpdateAsync(SessionType sessionType)
    {
        _context.Entry(sessionType).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var sessionType = await _context.SessionTypes.FindAsync(id);
        if (sessionType != null)
        {
            sessionType.Status = GeneralStatus.Inactive;
            await _context.SaveChangesAsync();
        }
    }
}
