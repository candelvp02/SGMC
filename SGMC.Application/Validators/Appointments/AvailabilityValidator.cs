using SGMC.Application.Dto.Appointments;
using SGMC.Domain.Base;

namespace SGMC.Application.Validators.Appointments
{
    // Validador para DTOs de Availability
    public static class AvailabilityValidator
    {
        // Valida CreateAvailabilityDto
        public static OperationResult IsValidDto(this CreateAvailabilityDto dto)
        {
            var errores = new List<string>();

            if (dto.DoctorId <= 0)
                errores.Add("El ID del doctor es requerido.");

            if (dto.DayOfWeek < 0 || dto.DayOfWeek > 6)
                errores.Add("El día de la semana es inválido (debe ser 0-6).");

            if (dto.StartTime >= dto.EndTime)
                errores.Add("La hora de inicio debe ser menor que la hora de fin.");

            if (dto.StartTime.Hours < 7 || dto.EndTime.Hours > 19)
                errores.Add("El horario de disponibilidad debe estar entre 07:00 y 19:00.");

            return errores.Count > 0
                ? OperationResult.Fallo("Errores de validación de disponibilidad.", errores)
                : OperationResult.Exito();
        }

        // Valida UpdateAvailabilityDto
        public static OperationResult IsValidDto(this UpdateAvailabilityDto dto)
        {
            var errores = new List<string>();

            if (dto.AvailabilityId <= 0)
                errores.Add("El ID de disponibilidad es inválido.");

            if (dto.DoctorId <= 0)
                errores.Add("El ID del doctor es requerido.");

            if (dto.DayOfWeek < 0 || dto.DayOfWeek > 6)
                errores.Add("El día de la semana es inválido (debe ser 0-6).");

            if (dto.StartTime >= dto.EndTime)
                errores.Add("La hora de inicio debe ser menor que la hora de fin.");

            if (dto.StartTime.Hours < 7 || dto.EndTime.Hours > 19)
                errores.Add("El horario de disponibilidad debe estar entre 07:00 y 19:00.");

            return errores.Count > 0
                ? OperationResult.Fallo("Errores de validación de actualización de disponibilidad.", errores)
                : OperationResult.Exito();
        }
    }
}