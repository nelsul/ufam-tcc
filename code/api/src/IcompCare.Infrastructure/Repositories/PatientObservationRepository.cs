using IcompCare.Domain.Entities;
using IcompCare.Domain.Enums;
using IcompCare.Domain.Interfaces;
using IcompCare.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IcompCare.Infrastructure.Repositories;

public class PatientObservationRepository : IPatientObservationRepository
{
    private readonly IcompCareDbContext _context;

    public PatientObservationRepository(IcompCareDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<PatientObservation> Items, int TotalCount)> GetAllAsync(
        int pageNumber,
        int pageSize
    )
    {
        var query = _context
            .PatientObservations.Include(po => po.Student)
            .Include(po => po.Observation)
            .Include(po => po.Professional)
            .Where(po => po.Status == GeneralStatus.Active);

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(po => po.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<PatientObservation?> GetByIdAsync(long id)
    {
        return await _context
            .PatientObservations.Include(po => po.Student)
            .Include(po => po.Observation)
            .Include(po => po.Professional)
            .FirstOrDefaultAsync(po => po.Id == id && po.Status == GeneralStatus.Active);
    }

    public async Task<PatientObservation?> GetByPublicIdAsync(Guid publicId)
    {
        return await _context
            .PatientObservations.Include(po => po.Student)
            .Include(po => po.Observation)
            .Include(po => po.Professional)
            .FirstOrDefaultAsync(po =>
                po.PublicId == publicId && po.Status == GeneralStatus.Active
            );
    }

    public async Task<IEnumerable<PatientObservation>> GetByStudentIdAsync(long studentId)
    {
        return await _context
            .PatientObservations.Include(po => po.Student)
            .Include(po => po.Observation)
            .Include(po => po.Professional)
            .Where(po => po.StudentId == studentId && po.Status == GeneralStatus.Active)
            .OrderByDescending(po => po.CreatedAt)
            .ToListAsync();
    }

    public async Task<PatientObservation> AddAsync(PatientObservation patientObservation)
    {
        _context.PatientObservations.Add(patientObservation);
        await _context.SaveChangesAsync();
        return patientObservation;
    }

    public async Task UpdateAsync(PatientObservation patientObservation)
    {
        _context.Entry(patientObservation).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var patientObservation = await _context.PatientObservations.FindAsync(id);
        if (patientObservation != null)
        {
            patientObservation.Status = GeneralStatus.Inactive;
            await _context.SaveChangesAsync();
        }
    }
}
