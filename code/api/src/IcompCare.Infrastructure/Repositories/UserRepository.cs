using IcompCare.Domain.Entities;
using IcompCare.Domain.Enums;
using IcompCare.Domain.Interfaces;
using IcompCare.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IcompCare.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IcompCareDbContext _context;

    public UserRepository(IcompCareDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<User>> GetAllAsync(bool includeInactive = false)
    {
        var query = _context.Users.AsQueryable();
        if (!includeInactive)
        {
            query = query.Where(u => u.Status == UserStatus.Active);
        }
        return await query.ToListAsync();
    }

    public async Task<(IEnumerable<User> Items, int TotalCount)> GetAllAsync(
        int pageNumber,
        int pageSize,
        bool includeInactive = false
    )
    {
        var query = _context.Users.AsQueryable();
        if (!includeInactive)
        {
            query = query.Where(u => u.Status == UserStatus.Active);
        }

        var totalCount = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return (items, totalCount);
    }

    public async Task<(IEnumerable<User> Items, int TotalCount)> GetByRoleAsync(
        UserRole role,
        int pageNumber,
        int pageSize,
        bool includeInactive = false,
        string? search = null
    )
    {
        var query = _context.Users.Where(u => u.Role == role);
        if (!includeInactive)
        {
            query = query.Where(u => u.Status == UserStatus.Active);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(u =>
                u.Name.ToLower().Contains(searchLower)
                || u.InstitutionalEmail.ToLower().Contains(searchLower)
                || u.Registration.ToLower().Contains(searchLower)
            );
        }

        var totalCount = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return (items, totalCount);
    }

    public async Task<IEnumerable<User>> GetByRoleAsync(UserRole role, bool includeInactive = false)
    {
        var query = _context.Users.Where(u => u.Role == role);
        if (!includeInactive)
        {
            query = query.Where(u => u.Status == UserStatus.Active);
        }
        return await query.ToListAsync();
    }

    public async Task<User?> GetByIdAsync(long id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User?> GetByPublicIdAsync(Guid publicId)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.PublicId == publicId);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.InstitutionalEmail == email);
    }

    public async Task<User?> GetByRegistrationAsync(string registration)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Registration == registration);
    }

    public async Task<User> AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task UpdateAsync(User user)
    {
        _context.Entry(user).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            user.Status = UserStatus.Inactive;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<User>> GetActiveUsersByRoleAsync(
        UserRole role,
        string? search = null
    )
    {
        var query = _context.Users.Where(u => u.Role == role && u.Status == UserStatus.Active);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(u => u.Name.ToLower().Contains(searchLower));
        }

        return await query.ToListAsync();
    }
}
