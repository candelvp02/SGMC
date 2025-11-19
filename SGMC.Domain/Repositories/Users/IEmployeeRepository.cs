using SGMC.Domain.Entities.Users;

namespace SGMC.Domain.Repositories.Users
{
    public interface IEmployeeRepository
    {
        Task<Employee?> GetByIdAsync(int employeeId);
        Task<IEnumerable<Employee>> GetAllAsync();
        Task<Employee> AddAsync(Employee employee);
        Task UpdateAsync(Employee employee);
        Task DeleteAsync(int employeeId);
        Task<IEnumerable<Employee>> GetActiveEmployeesAsync();
        Task<IEnumerable<Employee>> GetByJobTitleAsync(string jobTitle);
        Task<bool> ExistsAsync(int employeeId);
    }
}