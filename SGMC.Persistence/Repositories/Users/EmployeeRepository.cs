using Microsoft.EntityFrameworkCore;
using SGMC.Domain.Entities.Users;
using SGMC.Domain.Repositories.Users;
using SGMC.Persistence.Base;
using SGMC.Persistence.Context;

namespace SGMC.Persistence.Repositories.Users
{
    public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(HealtSyncContext context) : base(context) { }

        public override async Task<Employee?> GetByIdAsync(int employeeId)
            => await _dbSet.FindAsync(employeeId);

        public async Task<IEnumerable<Employee>> GetActiveEmployeesAsync()
            => await _dbSet.Where(e => e.IsActive).ToListAsync();

        public async Task<IEnumerable<Employee>> GetByJobTitleAsync(string jobTitle)
            => await _dbSet.Where(e => e.JobTitle == jobTitle && e.IsActive).ToListAsync();

        public async Task<bool> ExistsAsync(int employeeId)
            => await _dbSet.AnyAsync(e => e.EmployeeId == employeeId);

        public override async Task DeleteAsync(int employeeId)
        {
            var entity = await GetByIdAsync(employeeId);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}