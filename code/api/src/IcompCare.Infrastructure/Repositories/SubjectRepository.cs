using IcompCare.Domain.Entities;
using IcompCare.Domain.Enums;
using IcompCare.Domain.Interfaces;
using IcompCare.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IcompCare.Infrastructure.Repositories;

public class SubjectRepository : ISubjectRepository
{
    private readonly IcompCareDbContext _context;

    public SubjectRepository(IcompCareDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Subject>> GetAllAsync(bool includeInactive = false)
    {
        var query = _context.Subjects.AsQueryable();
        if (!includeInactive)
        {
            query = query.Where(s => s.Status == GeneralStatus.Active);
        }
        return await query.ToListAsync();
    }

    public async Task<(IEnumerable<Subject> Items, int TotalCount)> GetAllAsync(
        int pageNumber,
        int pageSize,
        bool includeInactive = false,
        string? search = null
    )
    {
        var query = _context.Subjects.AsQueryable();
        if (!includeInactive)
        {
            query = query.Where(s => s.Status == GeneralStatus.Active);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(s => s.Name.Contains(search) || s.Code.Contains(search));
        }

        var totalCount = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return (items, totalCount);
    }

    public async Task<Subject?> GetByIdAsync(long id)
    {
        return await _context.Subjects.FindAsync(id);
    }

    public async Task<Subject?> GetByPublicIdAsync(Guid publicId)
    {
        return await _context.Subjects.FirstOrDefaultAsync(s => s.PublicId == publicId);
    }

    public async Task<Subject?> GetByCodeAsync(string code)
    {
        return await _context.Subjects.FirstOrDefaultAsync(s => s.Code == code);
    }

    public async Task<Subject> AddAsync(Subject subject)
    {
        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();
        return subject;
    }

    public async Task UpdateAsync(Subject subject)
    {
        _context.Entry(subject).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var subject = await _context.Subjects.FindAsync(id);
        if (subject != null)
        {
            subject.Status = GeneralStatus.Inactive;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Subject>> GetActiveSubjectsAsync(string? search = null)
    {
        var query = _context.Subjects.Where(s => s.Status == GeneralStatus.Active);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(s =>
                s.Name.ToLower().Contains(searchLower) || s.Code.ToLower().Contains(searchLower)
            );
        }

        return await query.ToListAsync();
    }
}
