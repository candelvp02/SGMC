using Microsoft.EntityFrameworkCore;
using SGMC.Domain.Entities.System;
using SGMC.Domain.Repositories.System;
using SGMC.Persistence.Base;
using SGMC.Persistence.Context;

namespace SGMC.Persistence.Repositories.System
{
    public sealed class StatusRepository : BaseRepository<Status>, IStatusRepository
    {
        public StatusRepository(HealtSyncContext context) : base(context) { }

        public async Task<Status?> GetByNameAsync(string statusName)
            => await _dbSet.FirstOrDefaultAsync(s => s.StatusName == statusName);

        public async Task<bool> ExistsAsync(int statusId)
            => await _dbSet.AnyAsync(s => s.StatusId == statusId);

        public async Task<bool> ExistsByNameAsync(string statusName)
            => await _dbSet.AnyAsync(s => s.StatusName == statusName);

        Task IStatusRepository.DeleteAsync(int statusId)
        {
            return DeleteAsync(statusId);
        }

        public override async Task<Status?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }
    }
}
