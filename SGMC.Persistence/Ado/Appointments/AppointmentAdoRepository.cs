using Microsoft.Extensions.Logging;
using SGMC.Persistence.Common;
using System.Data;
using SGMC.Application.Dto.Appointments;
using SGMC.Application.Interfaces.Persistence;

namespace SGMC.Persistence.Ado.Appointments
{
    public class AppointmentAdoRepository : IAppointmentAdoRepository
    {
        private readonly StoredProcedureExecutor _sp;
        private readonly ILogger<AppointmentAdoRepository> _logger;

        public AppointmentAdoRepository(StoredProcedureExecutor sp, ILogger<AppointmentAdoRepository> logger)
        {
            _sp = sp;
            _logger = logger;
        }

        public async Task<List<AppointmentDto>> ListWithDetailsAsync()
        {
            var appointments = new List<AppointmentDto>();
            try
            {
                using var r = await _sp.ExecuteReaderAsync("appointments.usp_Appointment_ListWithDetails");
                while (await r.ReadAsync())
                {
                    appointments.Add(new AppointmentDto
                    {
                        AppointmentId = r.GetInt32("AppointmentID"),
                        PatientId = r.GetInt32("PatientID"),
                        PatientName = r.GetString("PatientName"),
                        DoctorId = r.GetInt32("DoctorID"),
                        DoctorName = r.GetString("DoctorName"),
                        AppointmentDate = r.GetDateTime("AppointmentDate"),
                        StatusId = r.GetInt32("StatusID"),
                        StatusName = r.GetString("StatusName"),
                        CreatedAt = r.GetDateTime("CreatedAt")
                    });
                }
                return appointments;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing appointments with details");
                return new List<AppointmentDto>();
            }
        }

        public async Task<List<AppointmentDto>> ListByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var appointments = new List<AppointmentDto>();
            try
            {
                using var r = await _sp.ExecuteReaderAsync(
                    "appointments.usp_Appointment_ListByDateRange",
                    ("@StartDate", startDate),
                    ("@EndDate", endDate)
                );
                while (await r.ReadAsync())
                {
                    appointments.Add(new AppointmentDto
                    {
                        AppointmentId = r.GetInt32("AppointmentID"),
                        PatientId = r.GetInt32("PatientID"),
                        PatientName = r.GetString("PatientName"),
                        DoctorId = r.GetInt32("DoctorID"),
                        DoctorName = r.GetString("DoctorName"),
                        AppointmentDate = r.GetDateTime("AppointmentDate"),
                        StatusId = r.GetInt32("StatusID"),
                        StatusName = r.GetString("StatusName"),
                        CreatedAt = r.GetDateTime("CreatedAt")
                    });
                }
                return appointments;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing appointments by date range");
                return new List<AppointmentDto>();
            }
        }

        public async Task<bool> CancelAsync(int appointmentId, string reason)
        {
            try
            {
                var result = await _sp.ExecuteNonQueryAsync(
                    "appointments.usp_Appointment_Cancel",
                    ("@AppointmentID", appointmentId)
                );
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error canceling appointment {AppointmentId}", appointmentId);
                return false;
            }
        }

        public async Task<bool> ConfirmAsync(int appointmentId)
        {
            try
            {
                var result = await _sp.ExecuteNonQueryAsync(
                    "appointments.usp_Appointment_Confirm",
                    ("@AppointmentID", appointmentId)
                );
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming appointment {AppointmentId}", appointmentId);
                return false;
            }
        }

        public async Task<bool> ExistsInTimeSlotAsync(int doctorId, DateTime appointmentDate)
        {
            try
            {
                var result = await _sp.ExecuteScalarAsync<int?>(
                    "appointments.usp_Appointment_ExistsInTimeSlot",
                    ("@DoctorID", doctorId),
                    ("@AppointmentDate", appointmentDate)
                );
                return result.HasValue && result.Value > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking appointment time slot for doctor {DoctorId}", doctorId);
                return false;
            }
        }
    }
}