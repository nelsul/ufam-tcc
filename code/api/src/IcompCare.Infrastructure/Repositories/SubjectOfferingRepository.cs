using IcompCare.Domain.Entities;
using IcompCare.Domain.Enums;
using IcompCare.Domain.Interfaces;
using IcompCare.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IcompCare.Infrastructure.Repositories;

public class SubjectOfferingRepository : ISubjectOfferingRepository
{
    private readonly IcompCareDbContext _context;

    public SubjectOfferingRepository(IcompCareDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<SubjectOffering> Items, int TotalCount)> GetAllAsync(
        int pageNumber,
        int pageSize,
        bool includeInactive = false,
        string? search = null
    )
    {
        var query = _context
            .SubjectOfferings.Include(so => so.Semester)
            .Include(so => so.Subject)
            .Include(so => so.Professor)
            .AsQueryable();

        if (!includeInactive)
        {
            query = query.Where(so => so.Status == GeneralStatus.Active);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.ToLower();
            query = query.Where(so =>
                so.Subject.Name.ToLower().Contains(search)
                || so.Subject.Code.ToLower().Contains(search)
                || so.Professor.Name.ToLower().Contains(search)
            );
        }

        var totalCount = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return (items, totalCount);
    }

    public async Task<SubjectOffering?> GetByPublicIdAsync(Guid publicId)
    {
        return await _context
            .SubjectOfferings.Include(so => so.Semester)
            .Include(so => so.Subject)
            .Include(so => so.Professor)
            .FirstOrDefaultAsync(so => so.PublicId == publicId);
    }

    public async Task<SubjectOffering?> GetByUniqueKeyAsync(
        long semesterId,
        long subjectId,
        long professorId
    )
    {
        return await _context.SubjectOfferings.FirstOrDefaultAsync(so =>
            so.SemesterId == semesterId
            && so.SubjectId == subjectId
            && so.ProfessorId == professorId
        );
    }

    public async Task<SubjectOffering> AddAsync(SubjectOffering subjectOffering)
    {
        _context.SubjectOfferings.Add(subjectOffering);
        await _context.SaveChangesAsync();
        return subjectOffering;
    }

    public async Task UpdateAsync(SubjectOffering subjectOffering)
    {
        _context.Entry(subjectOffering).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var subjectOffering = await _context.SubjectOfferings.FindAsync(id);
        if (subjectOffering != null)
        {
            subjectOffering.Status = GeneralStatus.Inactive;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<(
        IEnumerable<SubjectOffering> Items,
        int TotalCount
    )> GetActiveOfferingsByDateAsync(DateOnly date, int pageNumber, int pageSize)
    {
        var query = _context
            .SubjectOfferings.Include(so => so.Semester)
            .Include(so => so.Subject)
            .Include(so => so.Professor)
            .Where(so =>
                so.Status == GeneralStatus.Active
                && so.Subject.Status == GeneralStatus.Active
                && so.Professor.Status == UserStatus.Active
                && so.Semester.Status == GeneralStatus.Active
                && so.Semester.StartDate <= date
                && so.Semester.EndDate >= date
            );

        var totalCount = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return (items, totalCount);
    }

    public async Task<(IEnumerable<SubjectOffering> Items, int TotalCount)> GetByProfessorIdAsync(
        Guid professorPublicId,
        int pageNumber,
        int pageSize,
        bool includeInactive = false,
        string? search = null
    )
    {
        var query = _context
            .SubjectOfferings.Include(so => so.Semester)
            .Include(so => so.Subject)
            .Include(so => so.Professor)
            .Where(so => so.Professor.PublicId == professorPublicId);

        if (!includeInactive)
        {
            query = query.Where(so => so.Status == GeneralStatus.Active);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.ToLower();
            query = query.Where(so =>
                so.Subject.Name.ToLower().Contains(search)
                || so.Subject.Code.ToLower().Contains(search)
            );
        }

        var totalCount = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return (items, totalCount);
    }
}
