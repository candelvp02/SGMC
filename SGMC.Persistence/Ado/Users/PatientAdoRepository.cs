using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using SGMC.Application.Dto.Users;
using SGMC.Application.Interfaces.Persistence;
using SGMC.Persistence.Common;

namespace SGMC.Persistence.Ado.Users
{
    public class PatientAdoRepository : IPatientAdoRepository
    {
        private readonly StoredProcedureExecutor _sp;
        private readonly ILogger<PatientAdoRepository> _logger;

        public PatientAdoRepository(StoredProcedureExecutor sp, ILogger<PatientAdoRepository> logger)
        {
            _sp = sp;
            _logger = logger;
        }

        public async Task<List<PatientDto>> ListActiveAsync()
        {
            var patients = new List<PatientDto>();
            try
            {
                using var r = await _sp.ExecuteReaderAsync("users.usp_Patient_ListActive");
                while (await r.ReadAsync())
                {
                    patients.Add(MapPatientDto(r));
                }
                return patients;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing active patients");
                return new List<PatientDto>();
            }
        }

        public async Task<List<PatientDto>> SearchByNameAsync(string name)
        {
            var patients = new List<PatientDto>();
            try
            {
                using var r = await _sp.ExecuteReaderAsync(
                    "users.usp_Patient_SearchByName",
                    ("@Name", name)
                );
                while (await r.ReadAsync())
                {
                    patients.Add(MapPatientDto(r));
                }
                return patients;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching patients by name: {Name}", name);
                return new List<PatientDto>();
            }
        }

        public async Task<int> GetTotalAppointmentsAsync(int patientId)
        {
            try
            {
                var result = await _sp.ExecuteScalarAsync<int?>(
                    "users.usp_Patient_GetTotalAppointments",
                    ("@PatientID", patientId)
                );
                return result ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total appointments for patient {PatientId}", patientId);
                return 0;
            }
        }

        public async Task<PatientDto?> GetByIdWithDetailsAsync(int patientId)
        {
            try
            {
                using var r = await _sp.ExecuteReaderAsync(
                    "users.usp_Patient_GetByIdWithDetails",
                    ("@PatientID", patientId)
                );

                if (await r.ReadAsync())
                {
                    return MapPatientDto(r);
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting patient by ID with details: {PatientId}", patientId);
                return null;
            }
        }

        public async Task<PatientDto?> GetByIdentificationNumberAsync(string identificationNumber)
        {
            try
            {
                using var r = await _sp.ExecuteReaderAsync(
                    "users.usp_Patient_GetByIdentificationNumber",
                    ("@IdentificationNumber", identificationNumber)
                );

                if (await r.ReadAsync())
                {
                    return MapPatientDto(r);
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting patient by identification number: {IdentificationNumber}", identificationNumber);
                return null;
            }
        }

        public async Task<List<PatientDto>> ListByInsuranceProviderAsync(int insuranceProviderId)
        {
            var patients = new List<PatientDto>();
            try
            {
                using var r = await _sp.ExecuteReaderAsync(
                    "users.usp_Patient_ListByInsuranceProvider",
                    ("@InsuranceProviderID", insuranceProviderId)
                );
                while (await r.ReadAsync())
                {
                    patients.Add(MapPatientDto(r));
                }
                return patients;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing patients by insurance provider {InsuranceProviderId}", insuranceProviderId);
                return new List<PatientDto>();
            }
        }

        public async Task<bool> UpdateInsuranceProviderAsync(int patientId, int insuranceProviderId)
        {
            try
            {
                var result = await _sp.ExecuteNonQueryAsync(
                    "users.usp_Patient_UpdateInsuranceProvider",
                    ("@PatientID", patientId),
                    ("@InsuranceProviderID", insuranceProviderId)
                );
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating insurance provider for patient {PatientId}", patientId);
                return false;
            }
        }

        // Método privado helper para mapear SqlDataReader a PatientDto
        private static PatientDto MapPatientDto(SqlDataReader r)
        {
            return new PatientDto
            {
                PatientId = r.GetInt32(r.GetOrdinal("PatientID")),
                FirstName = r.GetString(r.GetOrdinal("FirstName")),
                LastName = r.GetString(r.GetOrdinal("LastName")),
                DateOfBirth = r.IsDBNull(r.GetOrdinal("DateOfBirth"))
                    ? null
                    : DateOnly.FromDateTime(r.GetDateTime(r.GetOrdinal("DateOfBirth"))),
                IdentificationNumber = r.GetString(r.GetOrdinal("IdentificationNumber")),
                Gender = r.GetString(r.GetOrdinal("Gender")),
                Email = r.IsDBNull(r.GetOrdinal("Email"))
                    ? string.Empty
                    : r.GetString(r.GetOrdinal("Email")),
                PhoneNumber = r.GetString(r.GetOrdinal("PhoneNumber")),
                Address = r.GetString(r.GetOrdinal("Address")),
                EmergencyContactName = r.GetString(r.GetOrdinal("EmergencyContactName")),
                EmergencyContactPhone = r.GetString(r.GetOrdinal("EmergencyContactPhone")),
                BloodType = r.GetString(r.GetOrdinal("BloodType")),
                Allergies = r.IsDBNull(r.GetOrdinal("Allergies"))
                    ? null
                    : r.GetString(r.GetOrdinal("Allergies")),
                InsuranceProviderId = r.GetInt32(r.GetOrdinal("InsuranceProviderID")),
                InsuranceProviderName = r.GetString(r.GetOrdinal("InsuranceProviderName")),
                IsActive = r.GetBoolean(r.GetOrdinal("IsActive"))
            };
        }
    }
}