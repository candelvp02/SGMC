using SGMC.Application.Dto.Users;

namespace SGMC.Web.Services
{
    public interface IPatientApiClient
    {
        Task<ApiResponse<List<PatientDto>>> GetAllAsync();
        Task<ApiResponse<PatientDto>> GetByIdAsync(int id);
        Task<ApiResponse<PatientDto>> CreateAsync(RegisterPatientDto dto);
        Task<ApiResponse<PatientDto>> UpdateAsync(UpdatePatientDto dto);
        Task<ApiResponse<bool>> DeleteAsync(int id);
    }
}