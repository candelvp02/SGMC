using SGMC.Application.Dto.Appointments;
using SGMC.Domain.Base;

namespace SGMC.Application.Interfaces.Service
{
    public interface IAvailabilityService
    {
        //gestion de disponibilidad rf3.1.7
        Task<OperationResult<AvailabilityDto>> CreateAsync(CreateAvailabilityDto dto);
        Task<OperationResult<AvailabilityDto>> UpdateAsync(UpdateAvailabilityDto dto);
        Task<OperationResult> DeleteAsync(int id);

        //consultas
        Task<OperationResult<AvailabilityDto>> GetByIdAsync(int id);
        Task<OperationResult<List<AvailabilityDto>>> GetByDoctorIdAsync(int doctorId);
        Task<OperationResult<List<AvailabilityDto>>> GetByDayOfWeekAsync(int doctorId, int dayOfWeek);
    }
}