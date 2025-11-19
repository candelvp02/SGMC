using SGMC.Domain.Entities.Medical;

namespace SGMC.Domain.Repositories.Medical
{
    public interface ISpecialtyRepository
    {
        Task<Specialty?> GetByIdAsync(short specialtyId);
        Task<IEnumerable<Specialty>> GetAllAsync();
        Task<Specialty> AddAsync(Specialty specialty);
        Task UpdateAsync(Specialty specialty);
        Task DeleteAsync(short specialtyId);
        Task<IEnumerable<Specialty>> GetActiveSpecialtiesAsync();
        Task<Specialty?> GetNameAsync(string specialtyName);
        Task<bool> ExistsAsync(short specialtyId);
        Task<bool> ExistsByNameAsync(string specialtyName);
        Task<IEnumerable<Specialty>> GetActiveAsync();
        Task DeleteAsync(Specialty existing);
    }
}