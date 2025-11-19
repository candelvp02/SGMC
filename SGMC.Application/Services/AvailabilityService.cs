using Microsoft.Extensions.Logging;
using SGMC.Application.Dto.Appointments;
using SGMC.Application.Extensions;
using SGMC.Application.Interfaces.Service;
using SGMC.Domain.Base;
using SGMC.Domain.Entities.Appointments;
using SGMC.Domain.Repositories.Appointments;
using SGMC.Domain.Repositories.Users;

namespace SGMC.Application.Services
{
    public class AvailabilityService : IAvailabilityService
    {
        private readonly IDoctorAvailabilityRepository _repository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly ILogger<AvailabilityService> _logger;

        public AvailabilityService(
            IDoctorAvailabilityRepository repository,
            IDoctorRepository doctorRepository,
            ILogger<AvailabilityService> logger)
        {
            _repository = repository;
            _doctorRepository = doctorRepository;
            _logger = logger;
        }

        // metodos CRUD

        public async Task<OperationResult<AvailabilityDto>> CreateAsync(CreateAvailabilityDto dto)
        {
            if (dto is null) return OperationResult<AvailabilityDto>.Fallo("Datos de disponibilidad requeridos.");

            // validaciones de campo fuera de trycatch
            var validationResult = dto.IsValidDto();
            if (!validationResult.Exitoso)
                return OperationResult<AvailabilityDto>.Fallo(validationResult.Mensaje, validationResult.Errores);

            try
            {
                // validaciones de negocio
                if (!await _doctorRepository.ExistsAsync(d => d.DoctorId == dto.DoctorId))
                    return OperationResult<AvailabilityDto>.Fallo("El doctor no existe.");

                var conflictExists = await _repository.CheckForConflictAsync(
                    dto.DoctorId,
                    dto.DayOfWeek,
                    dto.StartTime,
                    dto.EndTime
                );

                if (conflictExists)
                    return OperationResult<AvailabilityDto>.Fallo("El horario entra en conflicto con una disponibilidad existente.");

                // create entity
                var availability = new DoctorAvailability
                {
                    DoctorId = dto.DoctorId,
                    DayOfWeek = dto.DayOfWeek,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                var created = _repository.AddAsync(availability);
                var dtoResult = MapToDto(created);

                return OperationResult<AvailabilityDto>.Exito(dtoResult, "Disponibilidad creada correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear disponibilidad.");
                return OperationResult<AvailabilityDto>.Fallo($"Error interno al crear disponibilidad: {ex.Message}");
            }
        }

        private AvailabilityDto MapToDto(Task created)
        {
            throw new NotImplementedException();
        }

        public async Task<OperationResult<AvailabilityDto>> UpdateAsync(UpdateAvailabilityDto dto)
        {
            if (dto is null) return OperationResult<AvailabilityDto>.Fallo("Datos de actualización requeridos.");

            // validaciones de campo fuera de trycatch
            var validationResult = dto.IsValidDto();
            if (!validationResult.Exitoso)
                return OperationResult<AvailabilityDto>.Fallo(validationResult.Mensaje, validationResult.Errores);

            try
            {
                var existing = await _repository.GetByIdAsync(dto.AvailabilityId);
                if (existing is null)
                    return OperationResult<AvailabilityDto>.Fallo("Disponibilidad no encontrada.");

                // validación de conflicto
                var conflictExists = await _repository.CheckForConflictExcludingCurrentAsync(
                    dto.AvailabilityId,
                    dto.DoctorId,
                    dto.DayOfWeek,
                    dto.StartTime,
                    dto.EndTime
                );

                if (conflictExists)
                    return OperationResult<AvailabilityDto>.Fallo("El nuevo horario entra en conflicto con otra disponibilidad.");

                // update entity
                existing.DayOfWeek = dto.DayOfWeek;
                existing.UpdatedAt = DateTime.Now;

                await _repository.UpdateAsync(existing);
                var dtoResult = MapToDto(existing);

                return OperationResult<AvailabilityDto>.Exito(dtoResult!, "Disponibilidad actualizada correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar disponibilidad {Id}", dto.AvailabilityId);
                return OperationResult<AvailabilityDto>.Fallo($"Error interno al actualizar disponibilidad: {ex.Message}");
            }
        }

        public async Task<OperationResult> DeleteAsync(int id)
        {
            if (id <= 0) return OperationResult.Fallo("ID de disponibilidad inválido.");

            try
            {
                var exists = await _repository.ExistsAsync(id);
                if (!exists) return OperationResult.Fallo("Disponibilidad no encontrada.");


                await _repository.DeleteAsync(id);
                return OperationResult.Exito("Disponibilidad eliminada correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar disponibilidad {Id}", id);
                return OperationResult.Fallo($"Error al eliminar disponibilidad: {ex.Message}");
            }
        }

        // metodos de consulta

        public async Task<OperationResult<AvailabilityDto>> GetByIdAsync(int id)
        {
            if (id <= 0) return OperationResult<AvailabilityDto>.Fallo("ID de disponibilidad inválido.");

            try
            {
                var availability = await _repository.GetByIdAsync(id);
                if (availability is null)
                    return OperationResult<AvailabilityDto>.Fallo("Disponibilidad no encontrada.");

                var dto = MapToDto(availability);
                return OperationResult<AvailabilityDto>.Exito(dto!, "Disponibilidad obtenida correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener disponibilidad {Id}", id);
                return OperationResult<AvailabilityDto>.Fallo($"Error al obtener disponibilidad: {ex.Message}");
            }
        }

        public async Task<OperationResult<List<AvailabilityDto>>> GetByDoctorIdAsync(int doctorId)
        {
            if (doctorId <= 0) return OperationResult<List<AvailabilityDto>>.Fallo("ID de doctor inválido.");

            try
            {
                var availability = await _repository.GetByDoctorIdAsync(doctorId);
                var dtoList = availability.Select(MapToDto).ToList();
                return OperationResult<List<AvailabilityDto>>.Exito(dtoList!, "Disponibilidad del doctor obtenida correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener disponibilidad del doctor {Id}", doctorId);
                return OperationResult<List<AvailabilityDto>>.Fallo($"Error al obtener disponibilidad: {ex.Message}");
            }
        }

        public async Task<OperationResult<List<AvailabilityDto>>> GetByDayOfWeekAsync(int doctorId, int dayOfWeek)
        {
            if (doctorId <= 0) return OperationResult<List<AvailabilityDto>>.Fallo("ID de doctor inválido.");
            if (dayOfWeek < 0 || dayOfWeek > 6) return OperationResult<List<AvailabilityDto>>.Fallo("Día de la semana inválido.");

            try
            {
                var availability = await _repository.GetByDoctorIdAndDayOfWeekAsync(doctorId, dayOfWeek);
                var dtoList = availability.Select(MapToDto).ToList();
                return OperationResult<List<AvailabilityDto>>.Exito(dtoList!, "Disponibilidad del doctor por día de la semana obtenida correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener disponibilidad del doctor {Id} para día {Day}", doctorId, dayOfWeek);
                return OperationResult<List<AvailabilityDto>>.Fallo($"Error al obtener disponibilidad: {ex.Message}");
            }
        }

        // private mapping

        private static AvailabilityDto? MapToDto(DoctorAvailability a)
        {
            if (a == null) return null;

            return new AvailabilityDto
            {
                AvailabilityId = a.AvailabilityId,
                DoctorId = a.DoctorId,
                DayOfWeek = a.DayOfWeek,
                IsActive = a.IsActive,
                CreatedAt = a.CreatedAt
            };
        }
    }
}