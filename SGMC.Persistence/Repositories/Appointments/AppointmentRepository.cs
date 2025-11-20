using Microsoft.EntityFrameworkCore;
using SGMC.Domain.Entities.Appointments;
using SGMC.Domain.Repositories.Appointments;
using SGMC.Persistence.Base;
using SGMC.Persistence.Context;

namespace SGMC.Persistence.Repositories.Appointments
{
    public sealed class AppointmentRepository : BaseRepository<Appointment>, IAppointmentRepository
    {
        public AppointmentRepository(HealtSyncContext context) : base(context) { }

        public override async Task<Appointment> AddAsync(Appointment appointment)
        {
            await _dbSet.AddAsync(appointment);
            await _context.SaveChangesAsync();

            _context.Entry(appointment).State = EntityState.Detached;

            return await GetByIdWithDetailsAsync(appointment.AppointmentId)
                ?? throw new InvalidOperationException(
                    $"No se pudo recargar la cita {appointment.AppointmentId} después de crearla");
        }

        public async Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId)
            => await _dbSet.Where(a => a.PatientId == patientId).ToListAsync();

        public async Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId)
        {
            return await _dbSet
                .Where(a => a.DoctorId == doctorId)
                .Include(a => a.Status)
                .Include(a => a.Patient).ThenInclude(p => p!.PatientNavigation!)
                .Include(a => a.Doctor).ThenInclude(d => d!.DoctorNavigation!)
                .ToListAsync();
        }
        public async Task<IEnumerable<Appointment>> GetByDoctorIdWithDetailsAsync(int doctorId)
        {
            return await _dbSet
                .Where(a => a.DoctorId == doctorId)
                .Include(a => a.Status)
                .Include(a => a.Patient).ThenInclude(p => p!.PatientNavigation!)
                .Include(a => a.Doctor).ThenInclude(d => d!.DoctorNavigation!)
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByStatusIdAsync(int statusId)
        {
            return await _dbSet
                .Where(a => a.StatusId == statusId)
                .Include(a => a.Status)
                .Include(a => a.Patient).ThenInclude(p => p!.PatientNavigation!)
                .Include(a => a.Doctor).ThenInclude(d => d!.DoctorNavigation!)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(a => a.AppointmentDate.Date >= startDate.Date && a.AppointmentDate.Date <= endDate.Date)
                .Include(a => a.Status)
                .Include(a => a.Patient).ThenInclude(p => p!.PatientNavigation!)
                .Include(a => a.Doctor).ThenInclude(d => d!.DoctorNavigation!)
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetUpcomingAppointmentsAsync(int patientId)
        {
            var now = DateTime.Now;
            return await _dbSet
                .Where(a => a.PatientId == patientId && a.AppointmentDate > now)
                .Include(a => a.Status)
                .Include(a => a.Patient).ThenInclude(p => p!.PatientNavigation!)
                .Include(a => a.Doctor).ThenInclude(d => d!.DoctorNavigation!)
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int appointmentId)
            => await _dbSet.AnyAsync(a => a.AppointmentId == appointmentId);

        public override async Task<Appointment?> GetByIdAsync(int appointmentId)
            => await _dbSet.FindAsync(appointmentId);

        public async Task<Appointment?> GetByIdWithDetailsAsync(int appointmentId)
        {
            return await _dbSet
                .Include(a => a.Status)
                .Include(a => a.Patient).ThenInclude(p => p!.PatientNavigation!)
                .Include(a => a.Doctor).ThenInclude(d => d!.DoctorNavigation!)
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);
        }

        public async Task<IEnumerable<Appointment>> GetAllWithDetailsAsync()
        {
            try
            {
                var list = await _dbSet
                .Include(a => a.Status)
                .Include(a => a.Patient).ThenInclude(p => p!.PatientNavigation!)
                .Include(a => a.Doctor).ThenInclude(d => d!.DoctorNavigation!)
                .ToListAsync();

                return list;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving appointments with details: {ex.Message}");
                throw;
            }

            //return await _dbSet
            //    .Include(a => a.Status)
            //    .Include(a => a.Patient).ThenInclude(p => p!.PatientNavigation!)
            //    .Include(a => a.Doctor).ThenInclude(d => d!.DoctorNavigation!)
            //    .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByPatientIdWithDetailsAsync(int patientId)
        {
            return await _dbSet
                .Where(a => a.PatientId == patientId)
                .Include(a => a.Status)
                .Include(a => a.Patient).ThenInclude(p => p!.PatientNavigation!)
                .Include(a => a.Doctor).ThenInclude(d => d!.DoctorNavigation!)
                .ToListAsync();
        }

        public async Task<bool> ExistsInTimeSlotAsync(int doctorId, DateTime appointmentDate)
            => await _dbSet.AnyAsync(a => a.DoctorId == doctorId && a.AppointmentDate == appointmentDate);
    }
}
