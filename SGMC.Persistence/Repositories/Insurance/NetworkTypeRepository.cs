using Microsoft.EntityFrameworkCore;
using SGMC.Domain.Entities.Insurance;
using SGMC.Domain.Repositories.Insurance;
using SGMC.Persistence.Base;
using SGMC.Persistence.Context;

namespace SGMC.Persistence.Repositories.Insurance
{
    public sealed class NetworkTypeRepository : BaseRepository<NetworkType>, INetworkTypeRepository
    {
        public NetworkTypeRepository(HealtSyncContext context) : base(context) { }

        public async Task<IEnumerable<NetworkType?>> GetActiveNetworkTypesAsync()
            => await _dbSet.Where(n => n.IsActive).Cast<NetworkType?>().ToListAsync();

        public async Task<NetworkType?> GetByNameAsync(string name)
            => await _dbSet.FirstOrDefaultAsync(n => n.Name == name);

        public async Task<bool> ExistsAsync(int networkTypeId)
            => await _dbSet.AnyAsync(n => n.NetworkTypeId == networkTypeId);

        public override async Task<NetworkType?> GetByIdAsync(int networkTypeId)
            => await _dbSet.FindAsync(networkTypeId);

        Task INetworkTypeRepository.DeleteAsync(int networkTypeId)
        {
            return DeleteAsync(networkTypeId);
        }
    }
}
