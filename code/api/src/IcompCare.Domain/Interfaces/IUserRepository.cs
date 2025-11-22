using IcompCare.Domain.Entities;
using IcompCare.Domain.Enums;

namespace IcompCare.Domain.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync(bool includeInactive = false);
    Task<(IEnumerable<User> Items, int TotalCount)> GetAllAsync(
        int pageNumber,
        int pageSize,
        bool includeInactive = false
    );
    Task<IEnumerable<User>> GetByRoleAsync(UserRole role, bool includeInactive = false);
    Task<(IEnumerable<User> Items, int TotalCount)> GetByRoleAsync(
        UserRole role,
        int pageNumber,
        int pageSize,
        bool includeInactive = false,
        string? search = null
    );
    Task<User?> GetByIdAsync(long id);
    Task<User?> GetByPublicIdAsync(Guid publicId);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByRegistrationAsync(string registration);
    Task<User> AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(long id);
    Task<IEnumerable<User>> GetActiveUsersByRoleAsync(UserRole role, string? search = null);
}
