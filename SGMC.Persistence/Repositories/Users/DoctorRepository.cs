using Microsoft.EntityFrameworkCore;
using SGMC.Domain.Entities.Users;
using SGMC.Domain.Repositories.Users;
using SGMC.Persistence.Base;
using SGMC.Persistence.Context;

namespace SGMC.Persistence.Repositories.Users
{
    public class DoctorRepository : BaseRepository<Doctor>, IDoctorRepository
    {
        public DoctorRepository(HealtSyncContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Doctor>> GetAllAsync()
        {
            return await GetAllWithDetailsAsync();
        }

        public override async Task<Doctor> AddAsync(Doctor doctor)
        {
            await _dbSet.AddAsync(doctor);
            await _context.SaveChangesAsync();
            return doctor;
        }

        public override async Task UpdateAsync(Doctor doctor)
        {
            _context.Entry(doctor).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public override async Task DeleteAsync(int id)
        {
            var doctor = await _dbSet.FindAsync(id);
            if (doctor != null)
            {
                _dbSet.Remove(doctor);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Doctor?> GetByEmailAsync(string email)
        {
            return await _dbSet
                .Include(d => d.DoctorNavigation)
                    .ThenInclude(p => p!.User)
                .FirstOrDefaultAsync(d => d.DoctorNavigation != null
                                          && d.DoctorNavigation.User != null
                                          && d.DoctorNavigation.User.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<bool> ExistsByLicenseNumberAsync(string licenseNumber)
        {
            return await _dbSet.AnyAsync(d => d.LicenseNumber == licenseNumber);
        }

        public async Task<IEnumerable<Doctor>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .Where(d => d.IsActive)
                .Include(d => d.Specialty)
                .Include(d => d.DoctorNavigation)
                    .ThenInclude(p => p!.User)
                .ToListAsync();
        }

        public async Task<Doctor?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(d => d.Specialty)
                .Include(d => d.DoctorNavigation)
                    .ThenInclude(p => p!.User)
                .FirstOrDefaultAsync(d => d.DoctorId == id);
        }

        public async Task<IEnumerable<Doctor>> GetBySpecialtyIdAsync(short specialtyId)
        {
            return await _dbSet
                .Where(d => d.SpecialtyId == specialtyId && d.IsActive)
                .Include(d => d.DoctorNavigation)
                    .ThenInclude(p => p!.User)
                .Include(d => d.Specialty)
                .ToListAsync();
        }

        public async Task<IEnumerable<Doctor>> GetActiveDoctorsAsync()
        {
            return await _dbSet
                .Where(d => d.IsActive)
                .Include(d => d.DoctorNavigation)
                    .ThenInclude(p => p!.User)
                .Include(d => d.Specialty)
                .ToListAsync();
        }

        public async Task<Doctor?> GetByLicenseNumberAsync(string licenseNumber)
        {
            return await _dbSet
                .Include(d => d.DoctorNavigation)
                    .ThenInclude(p => p!.User)
                .Include(d => d.Specialty)
                .FirstOrDefaultAsync(d => d.LicenseNumber == licenseNumber);
        }
    }
}