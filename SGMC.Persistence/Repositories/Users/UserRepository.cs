using Microsoft.EntityFrameworkCore;
using SGMC.Domain.Entities.Users;
using SGMC.Domain.Repositories.Users;
using SGMC.Persistence.Base;
using SGMC.Persistence.Context;

namespace SGMC.Persistence.Repositories.Users
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(HealtSyncContext context) : base(context) { }
        public override async Task<User> AddAsync(User user)
        {
            await _dbSet.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User?> GetByEmailAsync(string email)
            => await _dbSet.FirstOrDefaultAsync(u => u.Email == email);

        public async Task<User?> GetByEmailAndPasswordAsync(string email, string password)
            => await _dbSet.FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash == password);

        public async Task<IEnumerable<User>> GetActiveUsersAsync()
            => await _dbSet.Where(u => u.IsActive).ToListAsync();

        public async Task<IEnumerable<User>> GetByRoleIdAsync(int roleId)
            => await _dbSet.Where(u => u.RoleId == roleId).ToListAsync();

        public async Task<User?> GetByIdWithDetailsAsync(int userId)
        {
            return await _dbSet
                .Include(u => u.UserNavigation)
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<User?> GetByEmailWithDetailsAsync(string email)
        {
            return await _dbSet
                .Include(u => u.UserNavigation)
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> ExistsAsync(int userId)
            => await _dbSet.AnyAsync(u => u.UserId == userId);

        public async Task<bool> ExistsByEmailAsync(string email)
            => await _dbSet.AnyAsync(u => u.Email == email);

        public override async Task DeleteAsync(int userId)
        {
            var user = await _dbSet.FindAsync(userId);
            if (user != null)
            {
                _dbSet.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<User>> GetAllWithRoleAsync()
        {
            return await _dbSet
                .Include(u => u.Role)
                .ToListAsync();
        }

        public async Task<User?> GetByIdWithRoleAsync(int userId)
        {
            return await _dbSet
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }
        public async Task<User?> GetByEmailWithRoleAsync(string email)
        {
            return await _dbSet
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<User?>> GetActiveAsync()
        {
            return await _dbSet
                .Where(u => u.IsActive)
                .ToListAsync();
        }
    }
}