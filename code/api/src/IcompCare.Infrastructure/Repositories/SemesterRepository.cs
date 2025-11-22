using IcompCare.Domain.Entities;
using IcompCare.Domain.Enums;
using IcompCare.Domain.Interfaces;
using IcompCare.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IcompCare.Infrastructure.Repositories;

public class SemesterRepository : ISemesterRepository
{
    private readonly IcompCareDbContext _context;

    public SemesterRepository(IcompCareDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Semester>> GetAllAsync(bool includeInactive = false)
    {
        var query = _context.Semesters.AsQueryable();
        if (!includeInactive)
        {
            query = query.Where(s => s.Status == GeneralStatus.Active);
        }
        return await query.ToListAsync();
    }

    public async Task<(IEnumerable<Semester> Items, int TotalCount)> GetAllAsync(
        int pageNumber,
        int pageSize,
        bool includeInactive = false,
        string? search = null
    )
    {
        var query = _context.Semesters.AsQueryable();
        if (!includeInactive)
        {
            query = query.Where(s => s.Status == GeneralStatus.Active);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(s => s.Name.Contains(search));
        }

        var totalCount = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return (items, totalCount);
    }

    public async Task<Semester?> GetByIdAsync(long id)
    {
        return await _context.Semesters.FindAsync(id);
    }

    public async Task<Semester?> GetByPublicIdAsync(Guid publicId)
    {
        return await _context.Semesters.FirstOrDefaultAsync(s => s.PublicId == publicId);
    }

    public async Task<Semester?> GetByNameAsync(string name)
    {
        return await _context.Semesters.FirstOrDefaultAsync(s => s.Name == name);
    }

    public async Task<Semester?> GetByDateAsync(DateOnly date)
    {
        return await _context.Semesters.FirstOrDefaultAsync(s =>
            s.StartDate <= date && s.EndDate >= date && s.Status == GeneralStatus.Active
        );
    }

    public async Task<Semester> AddAsync(Semester semester)
    {
        _context.Semesters.Add(semester);
        await _context.SaveChangesAsync();
        return semester;
    }

    public async Task UpdateAsync(Semester semester)
    {
        _context.Entry(semester).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var semester = await _context.Semesters.FindAsync(id);
        if (semester != null)
        {
            semester.Status = GeneralStatus.Inactive;
            await _context.SaveChangesAsync();
        }
    }
}
