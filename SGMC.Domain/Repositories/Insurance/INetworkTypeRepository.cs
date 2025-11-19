using SGMC.Domain.Entities.Insurance;

namespace SGMC.Domain.Repositories.Insurance
{
    public interface INetworkTypeRepository
    {
        Task<NetworkType?> GetByIdAsync(int networkTypeId);
        Task<IEnumerable<NetworkType>> GetAllAsync();
        Task<NetworkType> AddAsync(NetworkType networkType);
        Task UpdateAsync(NetworkType networkType);
        Task DeleteAsync(int networkTypeId);
        Task<IEnumerable<NetworkType?>> GetActiveNetworkTypesAsync();
        Task<NetworkType?> GetByNameAsync(string name);
        Task<bool> ExistsAsync(int networkTypeId);
    }
}