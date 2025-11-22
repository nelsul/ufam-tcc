using IcompCare.Domain.Entities;
using IcompCare.Domain.Enums;
using IcompCare.Domain.Interfaces;
using IcompCare.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IcompCare.Infrastructure.Repositories;

public class PatientRecordRepository : IPatientRecordRepository
{
    private readonly IcompCareDbContext _context;

    public PatientRecordRepository(IcompCareDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<PatientRecord> Items, int TotalCount)> GetAllAsync(
        int pageNumber,
        int pageSize
    )
    {
        var query = _context
            .PatientRecords.Include(pr => pr.Student)
            .Where(pr => pr.Status == GeneralStatus.Active);

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(pr => pr.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<PatientRecord?> GetByIdAsync(long id)
    {
        return await _context
            .PatientRecords.Include(pr => pr.Student)
            .FirstOrDefaultAsync(pr => pr.Id == id && pr.Status == GeneralStatus.Active);
    }

    public async Task<PatientRecord?> GetByPublicIdAsync(Guid publicId)
    {
        return await _context
            .PatientRecords.Include(pr => pr.Student)
            .FirstOrDefaultAsync(pr =>
                pr.PublicId == publicId && pr.Status == GeneralStatus.Active
            );
    }

    public async Task<IEnumerable<PatientRecord>> GetByStudentIdAsync(long studentId)
    {
        return await _context
            .PatientRecords.Include(pr => pr.Student)
            .Where(pr => pr.StudentId == studentId && pr.Status == GeneralStatus.Active)
            .OrderByDescending(pr => pr.CreatedAt)
            .ToListAsync();
    }

    public async Task<PatientRecord> AddAsync(PatientRecord patientRecord)
    {
        _context.PatientRecords.Add(patientRecord);
        await _context.SaveChangesAsync();
        return patientRecord;
    }

    public async Task UpdateAsync(PatientRecord patientRecord)
    {
        _context.Entry(patientRecord).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var patientRecord = await _context.PatientRecords.FindAsync(id);
        if (patientRecord != null)
        {
            patientRecord.Status = GeneralStatus.Inactive;
            await _context.SaveChangesAsync();
        }
    }
}
