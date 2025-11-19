using SGMC.Application.Dto.Appointments;
using SGMC.Application.Extensions;
using SGMC.Application.Interfaces.Service;
using SGMC.Domain.Base;
using SGMC.Domain.Entities.Appointments;
using SGMC.Domain.Repositories.Appointments;
using SGMC.Domain.Repositories.Users;
using Microsoft.Extensions.Logging;

namespace SGMC.Application.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _repository;
        private readonly IPatientRepository _patientRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly ILogger<AppointmentService> _logger;

        public AppointmentService(
            IAppointmentRepository repository,
            IPatientRepository patientRepository,
            IDoctorRepository doctorRepository,
            ILogger<AppointmentService> logger)
        {
            _repository = repository;
            _patientRepository = patientRepository;
            _doctorRepository = doctorRepository;
            _logger = logger;
        }

        // create
        public async Task<OperationResult<AppointmentDto>> CreateAsync(CreateAppointmentDto dto)
        {
            // validaciones de campo fuera de trycatch
            if (dto is null)
                return OperationResult<AppointmentDto>.Fallo("Los datos de la cita son requeridos.");

            // extension method para validaciones de campos 
            OperationResult validationResult = dto.IsValidDto();

            if (!validationResult.Exitoso)
                return OperationResult<AppointmentDto>.Fallo(validationResult.Mensaje, validationResult.Errores);

            try
            {
                // validaciones de ng que requieren acceso a bd

                var patientExists = await _patientRepository.ExistsAsync(dto.PatientId);
                if (!patientExists)
                    return OperationResult<AppointmentDto>.Fallo("El paciente no existe");

                var doctorExists = await _doctorRepository.ExistsAsync(d => d.DoctorId == dto.DoctorId);
                if (!doctorExists)
                    return OperationResult<AppointmentDto>.Fallo("El doctor no existe");

                var hasConflict = await _repository.ExistsInTimeSlotAsync(dto.DoctorId, dto.AppointmentDate);
                if (hasConflict)
                    return OperationResult<AppointmentDto>.Fallo("La cita entra en conflicto con otra existente");

                // create entity and save
                var appointment = new Appointment
                {
                    PatientId = dto.PatientId,
                    DoctorId = dto.DoctorId,
                    AppointmentDate = dto.AppointmentDate,
                    StatusId = 1, // Pending
                    CreatedAt = DateTime.Now
                };

                var created = await _repository.AddAsync(appointment);

                var dtoResult = MapToDto(created);

                return OperationResult<AppointmentDto>.Exito(dtoResult, "Cita creada correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la cita");
                return OperationResult<AppointmentDto>.Fallo("Error interno al crear la cita");
            }
        }

        // cancel
        public async Task<OperationResult> CancelAsync(int appointmentId)
        {
            if (appointmentId <= 0)
                return OperationResult.Fallo("El ID de la cita es inválido");

            try
            {
                var appointment = await _repository.GetByIdAsync(appointmentId);
                if (appointment is null)
                    return OperationResult.Fallo("La cita no existe");

                appointment.StatusId = 3; // 3 = cancelada
                appointment.UpdatedAt = DateTime.Now;

                await _repository.UpdateAsync(appointment);

                return OperationResult.Exito("Cita cancelada correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cancelar cita {Id}", appointmentId);
                return OperationResult.Fallo("Error al cancelar la cita");
            }
        }

        // confirm
        public async Task<OperationResult> ConfirmAsync(int appointmentId)
        {
            if (appointmentId <= 0)
                return OperationResult.Fallo("El ID de la cita es inválido");

            try
            {
                var appointment = await _repository.GetByIdAsync(appointmentId);
                if (appointment is null)
                    return OperationResult.Fallo("La cita no existe");

                appointment.StatusId = 2; // 2 = confirmada
                appointment.UpdatedAt = DateTime.Now;

                await _repository.UpdateAsync(appointment);

                return OperationResult.Exito("Cita confirmada correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al confirmar cita {Id}", appointmentId);
                return OperationResult.Fallo("Error al confirmar la cita");
            }
        }

        // reschedule
        public async Task<OperationResult> RescheduleAsync(int appointmentId, DateTime newDate)
        {
            if (appointmentId <= 0)
                return OperationResult.Fallo("El ID de la cita es inválido");

            try
            {
                var appointment = await _repository.GetByIdAsync(appointmentId);
                if (appointment is null)
                    return OperationResult.Fallo("La cita no existe");

                if (appointment.StatusId == 3)
                    return OperationResult.Fallo("No se puede reprogramar una cita cancelada");

                if (appointment.StatusId == 4)
                    return OperationResult.Fallo("No se puede reprogramar una cita completada");

                var hasConflict = await _repository.ExistsInTimeSlotAsync(appointment.DoctorId, newDate);
                if (hasConflict)
                    return OperationResult.Fallo("La nueva fecha entra en conflicto con otra cita");

                appointment.AppointmentDate = newDate;
                appointment.UpdatedAt = DateTime.Now;

                await _repository.UpdateAsync(appointment);

                return OperationResult.Exito("Cita reprogramada correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al reprogramar cita {Id}", appointmentId);
                return OperationResult.Fallo("Error al reprogramar la cita");
            }
        }

        // update
        public async Task<OperationResult<AppointmentDto>> UpdateAsync(UpdateAppointmentDto dto)
        {
            if (dto is null)
                return OperationResult<AppointmentDto>.Fallo("Los datos de la cita son requeridos.");

            // extension method para validaciones de campos 
            OperationResult validationResult = dto.IsValidDto();

            if (!validationResult.Exitoso)
                return OperationResult<AppointmentDto>.Fallo(validationResult.Mensaje, validationResult.Errores);

            try
            {
                var appointment = await _repository.GetByIdAsync(dto.AppointmentId);
                if (appointment is null)
                    return OperationResult<AppointmentDto>.Fallo("La cita no existe");

                appointment.AppointmentDate = dto.AppointmentDate;
                appointment.StatusId = dto.StatusId;
                appointment.UpdatedAt = DateTime.Now;

                await _repository.UpdateAsync(appointment);

                var dtoResult = MapToDto(appointment);
                return OperationResult<AppointmentDto>.Exito(dtoResult, "Cita actualizada correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar cita {Id}", dto.AppointmentId);
                return OperationResult<AppointmentDto>.Fallo("Error al actualizar la cita");
            }
        }

        // delete
        public async Task<OperationResult> DeleteAsync(int id)
        {
            if (id <= 0)
                return OperationResult.Fallo("El ID de la cita es inválido");

            try
            {
                var exists = await _repository.ExistsAsync(id);
                if (!exists)
                    return OperationResult.Fallo("La cita no existe");

                await _repository.DeleteAsync(id);
                return OperationResult.Exito("Cita eliminada correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar cita {Id}", id);
                return OperationResult.Fallo("Error al eliminar la cita");
            }
        }

        // queries
        public async Task<OperationResult<List<AppointmentDto>>> GetAllAsync()
        {
            try
            {
                var appointments = await _repository.GetAllWithDetailsAsync();
                var list = appointments.Select(MapToDto).ToList();
                return OperationResult<List<AppointmentDto>>.Exito(list, "Citas obtenidas correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener citas");
                return OperationResult<List<AppointmentDto>>.Fallo("Error al obtener las citas");
            }
        }

        public async Task<OperationResult<AppointmentDto>> GetByIdAsync(int id)
        {
            if (id <= 0)
                return OperationResult<AppointmentDto>.Fallo("El ID de la cita es inválido");

            try
            {
                var appointment = await _repository.GetByIdWithDetailsAsync(id);
                if (appointment is null)
                    return OperationResult<AppointmentDto>.Fallo("La cita no existe");

                var dto = MapToDto(appointment);
                return OperationResult<AppointmentDto>.Exito(dto, "Cita obtenida correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener cita {Id}", id);
                return OperationResult<AppointmentDto>.Fallo("Error al obtener la cita");
            }
        }

        public async Task<OperationResult<List<AppointmentDto>>> GetByPatientIdAsync(int patientId)
        {
            if (patientId <= 0)
                return OperationResult<List<AppointmentDto>>.Fallo("El ID del paciente es inválido");

            try
            {
                var appointments = await _repository.GetByPatientIdWithDetailsAsync(patientId);
                var list = appointments.Select(MapToDto).ToList();
                return OperationResult<List<AppointmentDto>>.Exito(list, "Citas obtenidas correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener citas de paciente {Id}", patientId);
                return OperationResult<List<AppointmentDto>>.Fallo("Error al obtener las citas del paciente");
            }
        }

        public async Task<OperationResult<List<AppointmentDto>>> GetByDoctorIdAsync(int doctorId)
        {
            if (doctorId <= 0)
                return OperationResult<List<AppointmentDto>>.Fallo("El ID del doctor es inválido");

            try
            {
                var appointments = await _repository.GetByDoctorIdWithDetailsAsync(doctorId);
                var list = appointments.Select(MapToDto).ToList();
                return OperationResult<List<AppointmentDto>>.Exito(list, "Citas obtenidas correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener citas del doctor {Id}", doctorId);
                return OperationResult<List<AppointmentDto>>.Fallo("Error al obtener las citas del doctor");
            }
        }

        public async Task<OperationResult<List<AppointmentDto>>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate)
                return OperationResult<List<AppointmentDto>>.Fallo("El rango de fechas es inválido");

            try
            {
                var appointments = await _repository.GetByDateRangeAsync(startDate, endDate);
                var list = appointments.Select(MapToDto).ToList();
                return OperationResult<List<AppointmentDto>>.Exito(list, "Citas obtenidas correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener citas en rango de fechas");
                return OperationResult<List<AppointmentDto>>.Fallo("Error al obtener las citas en el rango de fechas");
            }
        }

        public async Task<OperationResult<List<AppointmentDto>>> GetUpcomingForPatientAsync(int patientId)
        {
            if (patientId <= 0)
                return OperationResult<List<AppointmentDto>>.Fallo("El ID del paciente es inválido");

            try
            {
                var appointments = await _repository.GetUpcomingAppointmentsAsync(patientId);
                var list = appointments.Select(MapToDto).ToList();
                return OperationResult<List<AppointmentDto>>.Exito(list, "Citas obtenidas correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener próximas citas del paciente {Id}", patientId);
                return OperationResult<List<AppointmentDto>>.Fallo("Error al obtener las próximas citas del paciente");
            }
        }

        public async Task<OperationResult<List<AppointmentDto>>> GetFilteredAppointmentsAsync(AppointmentFilterDto filter)
        {
            if (filter is null)
                return OperationResult<List<AppointmentDto>>.Fallo("El filtro es requerido");

            try
            {
                IEnumerable<Appointment> appointments;

                if (filter.PatientId.HasValue)
                {
                    appointments = await _repository.GetByPatientIdAsync(filter.PatientId.Value);
                }
                else if (filter.DoctorId.HasValue)
                {
                    appointments = await _repository.GetByDoctorIdAsync(filter.DoctorId.Value);
                }
                else if (filter.StatusId.HasValue)
                {
                    appointments = await _repository.GetByStatusIdAsync(filter.StatusId.Value);
                }
                else
                {
                    appointments = await _repository.GetAllWithDetailsAsync();
                }

                var list = appointments.Select(MapToDto).ToList();
                return OperationResult<List<AppointmentDto>>.Exito(list, "Citas obtenidas correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener citas filtradas");
                return OperationResult<List<AppointmentDto>>.Fallo("Error al obtener las citas filtradas");
            }
        }

        // mapping
        private static AppointmentDto MapToDto(Appointment a)
        {
            if (a == null) throw new ArgumentNullException(nameof(a));

            return new AppointmentDto
            {
                AppointmentId = a.AppointmentId,
                PatientId = a.PatientId,
                DoctorId = a.DoctorId,
                AppointmentDate = a.AppointmentDate,
                StatusId = a.StatusId,
                PatientName = a.Patient?.PatientNavigation != null
                    ? $"{a.Patient.PatientNavigation.FirstName} {a.Patient.PatientNavigation.LastName}"
                    : string.Empty,
                DoctorName = a.Doctor?.DoctorNavigation != null
                    ? $"{a.Doctor.DoctorNavigation.FirstName} {a.Doctor.DoctorNavigation.LastName}"
                    : string.Empty,
                StatusName = a.Status?.StatusName ?? string.Empty,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt
            };
        }
    }
}