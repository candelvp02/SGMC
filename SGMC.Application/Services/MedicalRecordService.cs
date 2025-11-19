using Microsoft.Extensions.Logging;
using SGMC.Application.Dto.Medical;
using SGMC.Application.Extensions;
using SGMC.Application.Interfaces.Service;
using SGMC.Domain.Base;
using SGMC.Domain.Entities.Medical;
using SGMC.Domain.Repositories.Medical;
using SGMC.Domain.Repositories.Users;


namespace SGMC.Application.Services
{
    public class MedicalRecordService : IMedicalRecordService
    {
        private readonly IMedicalRecordRepository _repository;
        private readonly IPatientRepository _patientRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly ILogger<MedicalRecordService> _logger;

        public MedicalRecordService(
            IMedicalRecordRepository repository,
            IPatientRepository patientRepository,
            IDoctorRepository doctorRepository,
            ILogger<MedicalRecordService> logger)
        {
            _repository = repository;
            _patientRepository = patientRepository;
            _doctorRepository = doctorRepository;
            _logger = logger;
        }

        // metodos CRUD

        public async Task<OperationResult<MedicalRecordDto>> CreateAsync(CreateMedicalRecordDto dto)
        {
            if (dto is null) return OperationResult<MedicalRecordDto>.Fallo("Datos de registro médico requeridos.");

            // validaciones de campo fuera de trycatch
            var validationResult = dto.IsValidDto();
            if (!validationResult.Exitoso)
                return OperationResult<MedicalRecordDto>.Fallo(validationResult.Mensaje, validationResult.Errores);

            try
            {
                // validaciones de negocio
                if (!await _patientRepository.ExistsAsync(dto.PatientId))
                    return OperationResult<MedicalRecordDto>.Fallo("El paciente no existe.");

                if (!await _doctorRepository.ExistsAsync(d => d.DoctorId == dto.DoctorId))
                    return OperationResult<MedicalRecordDto>.Fallo("El doctor no existe.");

                // create entity
                var record = new MedicalRecord
                {
                    PatientId = dto.PatientId,
                    DoctorId = dto.DoctorId,
                    Diagnosis = dto.Diagnosis.Trim(),
                    Treatment = dto.Treatment.Trim(),
                    CreatedAt = DateTime.Now
                };

                var created = await _repository.AddAsync(record);
                var dtoResult = MapToDto(created);

                return OperationResult<MedicalRecordDto>.Exito(dtoResult!, "Registro médico creado correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear registro médico.");
                return OperationResult<MedicalRecordDto>.Fallo($"Error interno al crear registro: {ex.Message}");
            }
        }

        public async Task<OperationResult<MedicalRecordDto>> UpdateAsync(UpdateMedicalRecordDto dto)
        {
            if (dto is null) return OperationResult<MedicalRecordDto>.Fallo("Datos de actualización requeridos.");

            // validaciones de campo fuera de trycatch
            var validationResult = dto.IsValidDto();
            if (!validationResult.Exitoso)
                return OperationResult<MedicalRecordDto>.Fallo(validationResult.Mensaje, validationResult.Errores);

            try
            {
                var existing = await _repository.GetByIdAsync(dto.RecordId);
                if (existing is null)
                    return OperationResult<MedicalRecordDto>.Fallo("Registro médico no encontrado.");

                // update entity
                existing.Diagnosis = dto.Diagnosis.Trim();
                existing.Treatment = dto.Treatment.Trim();
                existing.UpdatedAt = DateTime.Now;

                await _repository.UpdateAsync(existing);
                var updated = await _repository.GetByIdWithDetailsAsync(existing.RecordId);
                var dtoResult = MapToDto(updated!);

                return OperationResult<MedicalRecordDto>.Exito(dtoResult!, "Registro médico actualizado correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar registro médico {Id}", dto.RecordId);
                return OperationResult<MedicalRecordDto>.Fallo($"Error interno al actualizar registro: {ex.Message}");
            }
        }

        public async Task<OperationResult> DeleteAsync(int id)
        {
            if (id <= 0) return OperationResult.Fallo("ID de registro médico inválido.");

            try
            {
                var exists = await _repository.ExistsAsync(id);
                if (!exists) return OperationResult.Fallo("Registro médico no encontrado.");

                await _repository.DeleteAsync(id);
                return OperationResult.Exito("Registro médico eliminado correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar registro médico {Id}", id);
                return OperationResult.Fallo($"Error al eliminar registro: {ex.Message}");
            }
        }

        // metodos de consulta

        public async Task<OperationResult<MedicalRecordDto>> GetByIdAsync(int id)
        {
            if (id <= 0) return OperationResult<MedicalRecordDto>.Fallo("ID de registro médico inválido.");

            try
            {
                var record = await _repository.GetByIdWithDetailsAsync(id);
                if (record is null)
                    return OperationResult<MedicalRecordDto>.Fallo("Registro médico no encontrado.");

                var dto = MapToDto(record);
                return OperationResult<MedicalRecordDto>.Exito(dto!, "Registro médico obtenido correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener registro médico {Id}", id);
                return OperationResult<MedicalRecordDto>.Fallo($"Error al obtener registro: {ex.Message}");
            }
        }

        public async Task<OperationResult<List<MedicalRecordDto>>> GetAllAsync()
        {
            try
            {
                var records = await _repository.GetAllWithDetailsAsync();
                var dtoList = records.Select(MapToDto).ToList();
                return OperationResult<List<MedicalRecordDto>>.Exito(dtoList!, "Registros médicos obtenidos correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los registros médicos.");
                return OperationResult<List<MedicalRecordDto>>.Fallo($"Error al obtener registros: {ex.Message}");
            }
        }

        public async Task<OperationResult<List<MedicalRecordDto>>> GetByPatientIdAsync(int patientId)
        {
            if (patientId <= 0) return OperationResult<List<MedicalRecordDto>>.Fallo("ID de paciente inválido.");

            try
            {
                var records = await _repository.GetByPatientIdAsync(patientId);
                var dtoList = records.Select(MapToDto).ToList();
                return OperationResult<List<MedicalRecordDto>>.Exito(dtoList!, "Registros médicos obtenidos por paciente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener registros médicos por paciente {Id}", patientId);
                return OperationResult<List<MedicalRecordDto>>.Fallo($"Error al obtener registros: {ex.Message}");
            }
        }

        public async Task<OperationResult<List<MedicalRecordDto>>> GetByDoctorIdAsync(int doctorId)
        {
            if (doctorId <= 0) return OperationResult<List<MedicalRecordDto>>.Fallo("ID de doctor inválido.");

            try
            {
                var records = await _repository.GetByDoctorIdAsync(doctorId);
                var dtoList = records.Select(MapToDto).ToList();
                return OperationResult<List<MedicalRecordDto>>.Exito(dtoList!, "Registros médicos obtenidos por doctor.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener registros médicos por doctor {Id}", doctorId);
                return OperationResult<List<MedicalRecordDto>>.Fallo($"Error al obtener registros: {ex.Message}");
            }
        }


        // private mapping
        private static MedicalRecordDto? MapToDto(MedicalRecord r)
        {
            if (r == null) return null;

            string patientName = $"{r.Patient?.PatientNavigation?.FirstName} {r.Patient?.PatientNavigation?.LastName}".Trim();
            string doctorName = $"{r.Doctor?.DoctorNavigation?.FirstName} {r.Doctor?.DoctorNavigation?.LastName}".Trim();

            return new MedicalRecordDto
            {
                RecordId = r.RecordId,
                PatientId = r.PatientId,
                DoctorId = r.DoctorId,
                Diagnosis = r.Diagnosis,
                Treatment = r.Treatment,
                PatientName = patientName,
                DoctorName = doctorName,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            };
        }
    }
}
