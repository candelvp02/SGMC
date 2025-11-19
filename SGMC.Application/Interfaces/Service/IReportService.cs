using SGMC.Application.Dto.Appointments;
using SGMC.Domain.Base;

namespace SGMC.Application.Interfaces.Service
{
    public interface IReportService
    {
        Task<OperationResult<List<AppointmentResultDto>>> GetFilteredAppointmentsAsync(ReportFilterDto filter);
        Task<OperationResult<byte[]>> GenerateAppointmentsReportAsync(ReportFilterDto filter);
        Task<OperationResult<byte[]>> GenerateExcelAppointmentsReportAsync(ReportFilterDto filter);
        Task<OperationResult<AppointmentStatisticsDto>> GetAppointmentStatisticsAsync(ReportFilterDto filter);
    }
}