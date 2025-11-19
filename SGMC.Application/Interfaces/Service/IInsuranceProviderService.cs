using SGMC.Application.Dto.Insurance;
using SGMC.Domain.Base;

namespace SGMC.Application.Interfaces.Service
{
    public interface IInsuranceProviderService
    {
        // CRUD
        Task<OperationResult<InsuranceProviderDto>> CreateAsync(CreateInsuranceProviderDto dto);
        Task<OperationResult<InsuranceProviderDto>> UpdateAsync(UpdateInsuranceProviderDto dto);
        Task<OperationResult> DeleteAsync(int id);

        // Queries
        Task<OperationResult<InsuranceProviderDto>> GetByIdAsync(int id);
        Task<OperationResult<List<InsuranceProviderDto>>> GetAllAsync();
        Task<OperationResult<List<InsuranceProviderDto>>> GetActiveAsync();
        Task<OperationResult<bool>> ExistsAsync(int id);
    }
}