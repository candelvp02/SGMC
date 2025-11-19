using SGMC.Application.Dto.Appointments;
using SGMC.Application.Dto.Common;
namespace SGMC.Web.Services
{
    public interface IAppointmentApiClient
    {
        Task<ApiResponse<List<AppointmentDto>>> GetAllAsync();
        Task<ApiResponse<AppointmentDto>> GetByIdAsync(int id);
        Task<ApiResponse<AppointmentDto>> CreateAsync(CreateAppointmentDto dto);
        Task<ApiResponse<AppointmentDto>> UpdateAsync(UpdateAppointmentDto dto);
        Task<ApiResponse<bool>> DeleteAsync(int id);
        Task<ApiResponse<List<AppointmentDto>>> GetByPatientAsync(int patientId);
        Task<ApiResponse<List<AppointmentDto>>> GetByDoctorAsync(int doctorId);
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public T? Data { get; set; }
    }
}