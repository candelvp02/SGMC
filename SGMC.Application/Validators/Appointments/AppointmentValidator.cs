using SGMC.Application.Dto.Appointments;
using SGMC.Domain.Base;

namespace SGMC.Application.Validators.Appointments
{
    // Validador para DTOs de Appointment
    public static class AppointmentValidator
    {
        // Valida CreateAppointmentDto
        public static OperationResult IsValidDto(this CreateAppointmentDto dto)
        {
            var errores = new List<string>();

            if (dto.PatientId <= 0)
                errores.Add("El ID del paciente es requerido.");

            if (dto.DoctorId <= 0)
                errores.Add("El ID del doctor es requerido.");

            if (dto.AppointmentDate == default)
                errores.Add("La fecha de la cita es requerida.");
            else if (dto.AppointmentDate < DateTime.Now.AddMinutes(-5))
                errores.Add("La fecha de la cita no puede ser en el pasado.");

            return errores.Count > 0
                ? OperationResult.Fallo("Errores de validación de cita.", errores)
                : OperationResult.Exito();
        }

        // Valida UpdateAppointmentDto
        public static OperationResult IsValidDto(this UpdateAppointmentDto dto)
        {
            var errores = new List<string>();

            if (dto.AppointmentId <= 0)
                errores.Add("El ID de la cita es inválido.");

            if (dto.StatusId <= 0)
                errores.Add("El estatus es requerido.");

            if (dto.AppointmentDate == default)
                errores.Add("La fecha de la cita es requerida.");

            return errores.Count > 0
                ? OperationResult.Fallo("Errores de validación de actualización de cita.", errores)
                : OperationResult.Exito();
        }

        // Valida ReportFilterDto
        public static OperationResult IsValidDto(this ReportFilterDto filter)
        {
            if (filter.StartDate.HasValue && filter.EndDate.HasValue &&
                filter.StartDate.Value > filter.EndDate.Value)
            {
                return OperationResult.Fallo("La fecha de inicio no puede ser mayor a la fecha de fin.");
            }

            return OperationResult.Exito();
        }
    }
}