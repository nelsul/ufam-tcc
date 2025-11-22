using IcompCare.Domain.Entities;
using IcompCare.Domain.Interfaces;
using IcompCare.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IcompCare.Infrastructure.Repositories;

public class StudentEnrollmentRepository : IStudentEnrollmentRepository
{
    private readonly IcompCareDbContext _context;

    public StudentEnrollmentRepository(IcompCareDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<StudentEnrollment>> GetAllAsync()
    {
        return await _context
            .StudentEnrollments.Include(e => e.Student)
            .Include(e => e.SubjectOffering)
                .ThenInclude(so => so.Subject)
            .Include(e => e.SubjectOffering)
                .ThenInclude(so => so.Semester)
            .ToListAsync();
    }

    public async Task<StudentEnrollment?> GetByIdAsync(long id)
    {
        return await _context
            .StudentEnrollments.Include(e => e.Student)
            .Include(e => e.SubjectOffering)
                .ThenInclude(so => so.Subject)
            .Include(e => e.SubjectOffering)
                .ThenInclude(so => so.Semester)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<StudentEnrollment?> GetByPublicIdAsync(Guid publicId)
    {
        return await _context
            .StudentEnrollments.Include(e => e.Student)
            .Include(e => e.SubjectOffering)
                .ThenInclude(so => so.Subject)
            .Include(e => e.SubjectOffering)
                .ThenInclude(so => so.Semester)
            .FirstOrDefaultAsync(e => e.PublicId == publicId);
    }

    public async Task<StudentEnrollment> AddAsync(StudentEnrollment entity)
    {
        await _context.StudentEnrollments.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(StudentEnrollment entity)
    {
        _context.StudentEnrollments.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(StudentEnrollment entity)
    {
        _context.StudentEnrollments.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<StudentEnrollment>> GetByStudentIdAsync(long studentId)
    {
        return await _context
            .StudentEnrollments.Include(e => e.Student)
            .Include(e => e.SubjectOffering)
                .ThenInclude(so => so.Subject)
            .Include(e => e.SubjectOffering)
                .ThenInclude(so => so.Semester)
            .Where(e => e.StudentId == studentId)
            .ToListAsync();
    }

    public async Task<IEnumerable<StudentEnrollment>> GetBySubjectOfferingIdAsync(
        long subjectOfferingId
    )
    {
        return await _context
            .StudentEnrollments.Include(e => e.Student)
            .Include(e => e.SubjectOffering)
                .ThenInclude(so => so.Subject)
            .Include(e => e.SubjectOffering)
                .ThenInclude(so => so.Semester)
            .Where(e => e.SubjectOfferingId == subjectOfferingId)
            .ToListAsync();
    }

    public async Task<StudentEnrollment?> GetByStudentAndOfferingAsync(
        long studentId,
        long subjectOfferingId
    )
    {
        return await _context
            .StudentEnrollments.Include(e => e.Student)
            .Include(e => e.SubjectOffering)
                .ThenInclude(so => so.Subject)
            .Include(e => e.SubjectOffering)
                .ThenInclude(so => so.Semester)
            .FirstOrDefaultAsync(e =>
                e.StudentId == studentId && e.SubjectOfferingId == subjectOfferingId
            );
    }

    public async Task<(
        IEnumerable<StudentEnrollment> Items,
        int TotalCount
    )> GetStudentsByOfferingPaginatedAsync(
        long subjectOfferingId,
        int pageNumber,
        int pageSize,
        string? search = null
    )
    {
        var query = _context
            .StudentEnrollments.Include(e => e.Student)
            .Include(e => e.SubjectOffering)
                .ThenInclude(so => so.Subject)
            .Include(e => e.SubjectOffering)
                .ThenInclude(so => so.Semester)
            .Where(e => e.SubjectOfferingId == subjectOfferingId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.ToLower();
            query = query.Where(e =>
                e.Student.Name.ToLower().Contains(search)
                || e.Student.FullName.ToLower().Contains(search)
                || e.Student.InstitutionalEmail.ToLower().Contains(search)
                || (
                    e.Student.Registration != null
                    && e.Student.Registration.ToLower().Contains(search)
                )
            );
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(e => e.Student.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<bool> IsStudentEnrolledWithProfessorAsync(long studentId, long professorId)
    {
        return await _context
            .StudentEnrollments.Include(e => e.SubjectOffering)
            .AnyAsync(e =>
                e.StudentId == studentId && e.SubjectOffering.ProfessorId == professorId
            );
    }
}
