using Microsoft.EntityFrameworkCore;
using SGMC.Domain.Entities.Appointments;
using SGMC.Domain.Repositories.Appointments;
using SGMC.Persistence.Base;
using SGMC.Persistence.Context;

namespace SGMC.Persistence.Repositories.Appointments
{
    public sealed class DoctorAvailabilityRepository : BaseRepository<DoctorAvailability>, IDoctorAvailabilityRepository
    {
        public DoctorAvailabilityRepository(HealtSyncContext context) : base(context) { }

        public override async Task<DoctorAvailability?> GetByIdAsync(int id)
            => await _dbSet.FindAsync(id);

        public async Task<IEnumerable<DoctorAvailability>> GetByDoctorIdAsync(int doctorId)
            => await _dbSet.Where(d => d.DoctorId == doctorId).ToListAsync();

        public async Task<IEnumerable<DoctorAvailability>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate)
            => await _dbSet.Where(d => d.AvailableDate >= startDate && d.AvailableDate <= endDate).ToListAsync();

        public async Task<IEnumerable<DoctorAvailability>> GetByDoctorAndDateRangeAsync(int doctorId, DateOnly startDate, DateOnly endDate)
            => await _dbSet.Where(d => d.DoctorId == doctorId && d.AvailableDate >= startDate && d.AvailableDate <= endDate).ToListAsync();

        public async Task<bool> IsAvailableAsync(int doctorId, DateOnly date, TimeOnly time)
        {
            return await _dbSet.AnyAsync(d =>
                d.DoctorId == doctorId &&
                d.AvailableDate == date &&
                d.StartTime <= time &&
                time < d.EndTime &&
                d.IsActive);
        }

        public async Task<bool> HasConflictAsync(int doctorId, DateOnly date, TimeOnly startTime, TimeOnly endTime)
        {
            return await _dbSet.AnyAsync(d =>
                d.DoctorId == doctorId &&
                d.AvailableDate == date &&
                d.IsActive &&
                !(d.EndTime <= startTime || d.StartTime >= endTime));
        }

        public new async Task AddAsync(DoctorAvailability availability)
        {
            await _dbSet.AddAsync(availability);
            await _context.SaveChangesAsync();
        }

        public new async Task UpdateAsync(DoctorAvailability availability)
        {
            _dbSet.Update(availability);
            await _context.SaveChangesAsync();
        }

        public override async Task DeleteAsync(int availabilityId)
        {
            var entity = await _dbSet.FindAsync(availabilityId);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        // Obtiene todas las disponibilidades de un doctor para un día de la semana específico.
        public async Task<IEnumerable<DoctorAvailability>> GetByDoctorIdAndDayOfWeekAsync(int doctorId, int dayOfWeek)
        {
            return await _dbSet
                .Where(d => d.DoctorId == doctorId)
                .ToListAsync();
        }

        // Verifica si existe un registro de disponibilidad con el ID dado.
        public async Task<bool> ExistsAsync(int id)
        {
            return await _dbSet.AnyAsync(d => d.Id == id);
        }

        public async Task<bool> CheckForConflictExcludingCurrentAsync(int availabilityId, int doctorId, int dayOfWeek, TimeSpan startTime, TimeSpan endTime)
        {
            return await _dbSet.AnyAsync(d =>
                d.Id != availabilityId && 
                d.DoctorId == doctorId);
        }

        public async Task<bool> CheckForConflictAsync(int doctorId, int dayOfWeek, TimeSpan startTime, TimeSpan endTime)
        {
            return await _dbSet.AnyAsync(d =>
                d.DoctorId == doctorId);
        }
    }
}