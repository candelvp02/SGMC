using SGMC.Domain.Entities.Medical;

namespace SGMC.Domain.Repositories.Medical
{
    public interface IAvailabilityModeRepository
    {
        Task<IEnumerable<AvailabilityMode>> GetActiveModesAsync();
        Task<AvailabilityMode?> GetByNameAsync(string name);
        Task<bool> ExistsAsync(short availabilityModeId);
        Task<AvailabilityMode?> GetByIdAsync(short availabilityModeId);
        Task DeleteAsync(short availabilityModeId);
    }
}