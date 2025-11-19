using SGMC.Application.Dto.Appointments;

namespace SGMC.Application.Interfaces.Persistence
{
    public interface IAppointmentAdoRepository
    {
        Task<List<AppointmentDto>> ListWithDetailsAsync();
        Task<List<AppointmentDto>> ListByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<bool> ConfirmAsync(int appointmentId);
        Task<bool> ExistsInTimeSlotAsync(int doctorId, DateTime appointmentDate);
    }
}