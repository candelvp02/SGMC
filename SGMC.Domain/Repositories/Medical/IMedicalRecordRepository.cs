using SGMC.Domain.Entities.Medical;

namespace SGMC.Domain.Repositories.Medical
{
    public interface IMedicalRecordRepository
    {
        // Operaciones CRUD
        Task<MedicalRecord> AddAsync(MedicalRecord record);
        Task<MedicalRecord> UpdateAsync(MedicalRecord record);
        Task<MedicalRecord?> GetByIdAsync(int id);

        // Consultas
        Task<IEnumerable<MedicalRecord>> GetByPatientIdAsync(int patientId);
        Task<IEnumerable<MedicalRecord>> GetByDoctorIdAsync(int doctorId);
        Task<MedicalRecord?> GetByIdAsync(string? id);
        Task<MedicalRecord?> GetByIdWithDetailsAsync(int recordId);
        Task<bool> ExistsAsync(int id);
        Task DeleteAsync(int id);
        Task<IEnumerable<MedicalRecord>> GetAllWithDetailsAsync();
    }
}
