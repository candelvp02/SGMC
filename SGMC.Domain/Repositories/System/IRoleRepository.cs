using SGMC.Domain.Entities.System;

namespace SGMC.Domain.Repositories.System
{
    public interface IRoleRepository
    {
        Task<Role?> GetByIdAsync(int roleId);
        Task<IEnumerable<Role>> GetAllAsync();
        Task<Role> AddAsync(Role role);
        Task UpdateAsync(Role role);
        Task DeleteAsync(int roleId);
        Task<IEnumerable<Role>> GetActiveRolesAsync();
        Task<Role?> GetByNameAsync(string roleName);
        Task<bool> ExistsAsync(int roleId);
        Task<bool> ExistsByNameAsync(string roleName);
    }
}