using SGMC.Application.Dto.Appointments;
using SGMC.Application.Dto.Insurance;
using SGMC.Application.Dto.Medical;
using SGMC.Application.Dto.System;
using SGMC.Application.Dto.Users;
using SGMC.Domain.Base;
using System.Text.RegularExpressions;

namespace SGMC.Application.Extensions
{
    public static class Validate
    {
        private static readonly Regex CedulaRe = new(@"^\d{3}-\d{7}-\d{1}$", RegexOptions.Compiled);
        private static readonly Regex EmailRe = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
        private static readonly Regex UpperRe = new(@"[A-Z]", RegexOptions.Compiled);
        private static readonly Regex LowerRe = new(@"[a-z]", RegexOptions.Compiled);
        private static readonly Regex DigitRe = new(@"\d", RegexOptions.Compiled);

        // validaciones de appointment
        public static OperationResult IsValidDto(this CreateAppointmentDto dto)
        {
            var errores = new List<string>();

            if (dto.PatientId <= 0) errores.Add("El ID del paciente es requerido.");
            if (dto.DoctorId <= 0) errores.Add("El ID del doctor es requerido.");

            if (dto.AppointmentDate == default)
                errores.Add("La fecha de la cita es requerida.");
            else if (dto.AppointmentDate < DateTime.Now.AddMinutes(-5))
                errores.Add("La fecha de la cita no puede ser en el pasado.");

            return errores.Count > 0 ? OperationResult.Fallo("Errores de validación de cita.", errores) : OperationResult.Exito();
        }

        public static OperationResult IsValidDto(this UpdateAppointmentDto dto)
        {
            var errores = new List<string>();
            if (dto.AppointmentId <= 0) errores.Add("El ID de la cita es inválido.");
            if (dto.StatusId <= 0) errores.Add("El estatus es requerido.");
            if (dto.AppointmentDate == default) errores.Add("La fecha de la cita es requerida.");

            return errores.Count > 0 ? OperationResult.Fallo("Errores de validación de actualización de cita.", errores) : OperationResult.Exito();
        }

        //validaciones de usuario
        public static OperationResult IsValidDto(this RegisterUserDto dto)
        {
            var errores = new List<string>();

            // Email validation
            if (string.IsNullOrWhiteSpace(dto.Email) || !EmailRe.IsMatch(dto.Email))
                errores.Add("Formato de email inválido.");

            // Password validation
            if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 8 ||
                !UpperRe.IsMatch(dto.Password) || !LowerRe.IsMatch(dto.Password) || !DigitRe.IsMatch(dto.Password))
                errores.Add("La contraseña debe tener al menos 8 caracteres, e incluir mayúsculas, minúsculas y números.");

            // RoleId validation
            if (dto.RoleId <= 0)
                errores.Add("El ID del rol es requerido.");

            return errores.Count > 0 ? OperationResult.Fallo("Errores de validación de registro de usuario.", errores) : OperationResult.Exito();
        }

        public static OperationResult IsValidDto(this UpdateUserDto dto)
        {
            var errores = new List<string>();

            if (dto.UserId <= 0)
                errores.Add("El ID del usuario es inválido.");

            // Email validation
            if (string.IsNullOrWhiteSpace(dto.Email) || !EmailRe.IsMatch(dto.Email))
                errores.Add("Formato de email inválido.");

            // RoleId validation
            if (dto.RoleId <= 0)
                errores.Add("El ID del rol es requerido.");

            return errores.Count > 0 ? OperationResult.Fallo("Errores de validación de actualización de usuario.", errores) : OperationResult.Exito();
        }

        // validaciones de paciente
        public static OperationResult IsValidDto(this RegisterPatientDto dto)
        {
            var errores = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.FirstName) || dto.FirstName.Length < 2 || dto.FirstName.Length > 40)
                errores.Add("El nombre debe tener entre 2 y 40 caracteres.");

            if (string.IsNullOrWhiteSpace(dto.LastName) || dto.LastName.Length < 2 || dto.LastName.Length > 40)
                errores.Add("El apellido debe tener entre 2 y 40 caracteres.");

            if (string.IsNullOrWhiteSpace(dto.IdentificationNumber) || !CedulaRe.IsMatch(dto.IdentificationNumber))
                errores.Add("Formato de cédula inválido. Debe ser XXX-XXXXXXX-X.");

            if (string.IsNullOrWhiteSpace(dto.Gender) || (dto.Gender != "M" && dto.Gender != "F"))
                errores.Add("El género debe ser 'M' o 'F'.");

            if (dto.DateOfBirth.HasValue)
            {
                var age = DateTime.Now.Year - dto.DateOfBirth.Value.Year;
                if (age < 0 || age > 120) errores.Add("La fecha de nacimiento no es válida.");
            }

            if (string.IsNullOrWhiteSpace(dto.Email) || !EmailRe.IsMatch(dto.Email))
                errores.Add("Formato de email inválido.");

            if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 8 ||
                !UpperRe.IsMatch(dto.Password) || !LowerRe.IsMatch(dto.Password) || !DigitRe.IsMatch(dto.Password))
                errores.Add("La contraseña debe tener al menos 8 caracteres, incluir mayúsculas, minúsculas y números.");

            if (dto.InsuranceProviderId <= 0)
                errores.Add("El proveedor de seguro es requerido.");

            return errores.Count > 0 ? OperationResult.Fallo("Errores de validación de paciente.", errores) : OperationResult.Exito();
        }

        public static OperationResult IsValidDto(this UpdatePatientDto dto)
        {
            var errores = new List<string>();
            if (dto.PatientId <= 0) errores.Add("El ID del paciente es inválido.");
            if (dto.InsuranceProviderId <= 0) errores.Add("El proveedor de seguro es requerido.");

            return errores.Count > 0 ? OperationResult.Fallo("Errores de validación de actualización de paciente.", errores) : OperationResult.Exito();
        }

        // validaciones de doctor
        public static OperationResult IsValidDto(this RegisterDoctorDto dto)
        {
            var errores = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.FirstName) || dto.FirstName.Length < 2 || dto.FirstName.Length > 40)
                errores.Add("El nombre debe tener entre 2 y 40 caracteres.");

            if (string.IsNullOrWhiteSpace(dto.LastName) || dto.LastName.Length < 2 || dto.LastName.Length > 40)
                errores.Add("El apellido debe tener entre 2 y 40 caracteres.");

            if (string.IsNullOrWhiteSpace(dto.IdentificationNumber) || !CedulaRe.IsMatch(dto.IdentificationNumber))
                errores.Add("Formato de cédula inválido.");

            if (string.IsNullOrWhiteSpace(dto.Gender) || (dto.Gender != "M" && dto.Gender != "F"))
                errores.Add("El género debe ser 'M' o 'F'.");

            if (string.IsNullOrWhiteSpace(dto.Email) || !EmailRe.IsMatch(dto.Email))
                errores.Add("Formato de email inválido.");

            if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 8 || dto.Password.Length > 100 ||
                !UpperRe.IsMatch(dto.Password) || !LowerRe.IsMatch(dto.Password) || !DigitRe.IsMatch(dto.Password))
                errores.Add("La contraseña debe ser segura (Mayúsculas, minúsculas, números).");

            if (dto.SpecialtyId <= 0)
                errores.Add("La especialidad es requerida.");

            return errores.Count > 0 ? OperationResult.Fallo("Errores de validación de doctor.", errores) : OperationResult.Exito();
        }

        public static OperationResult IsValidDto(this UpdateDoctorDto dto)
        {
            var errores = new List<string>();
            if (dto.DoctorId <= 0) errores.Add("El ID del doctor es inválido.");

            return errores.Count > 0 ? OperationResult.Fallo("Errores de validación de actualización de doctor.", errores) : OperationResult.Exito();
        }

        // validaciones de especialidad
        public static OperationResult IsValidDto(this CreateSpecialtyDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.SpecialtyName))
                return OperationResult.Fallo("El nombre de la especialidad es requerido.");

            return OperationResult.Exito();
        }

        public static OperationResult IsValidDto(this UpdateSpecialtyDto dto)
        {
            var errores = new List<string>();
            if (dto.SpecialtyId <= 0) errores.Add("El ID de la especialidad es inválido.");
            if (string.IsNullOrWhiteSpace(dto.SpecialtyName)) errores.Add("El nombre es requerido.");

            return errores.Count > 0 ? OperationResult.Fallo("Errores de validación de especialidad.", errores) : OperationResult.Exito();
        }
        // validaciones de seguro
        public static OperationResult IsValidDto(this CreateInsuranceProviderDto dto)
        {
            var errores = new List<string>();
            if (string.IsNullOrWhiteSpace(dto.Name)) errores.Add("El nombre es requerido.");
            if (string.IsNullOrWhiteSpace(dto.PhoneNumber)) errores.Add("El teléfono es requerido.");
            if (string.IsNullOrWhiteSpace(dto.Email)) errores.Add("Email inválido o requerido.");
            if (string.IsNullOrWhiteSpace(dto.Address)) errores.Add("La dirección es requerida.");
            if (dto.NetworkTypeId <= 0) errores.Add("El tipo de red es requerido.");

            return errores.Count > 0 ? OperationResult.Fallo("Errores de validación de proveedor de seguro.", errores) : OperationResult.Exito();
        }

        public static OperationResult IsValidDto(this UpdateInsuranceProviderDto dto)
        {
            var errores = new List<string>();
            if (dto.InsuranceProviderId <= 0) errores.Add("El ID del proveedor es inválido.");
            if (dto.NetworkTypeId <= 0) errores.Add("El tipo de red es requerido.");

            return errores.Count > 0 ? OperationResult.Fallo("Errores de validación de actualización de proveedor de seguro.", errores) : OperationResult.Exito();
        }

        // validaciones de reporte
        public static OperationResult IsValidDto(this ReportFilterDto filter)
        {
            if (filter.StartDate.HasValue && filter.EndDate.HasValue && filter.StartDate.Value > filter.EndDate.Value)
            {
                return OperationResult.Fallo("La fecha de inicio no puede ser mayor a la fecha de fin.");
            }
            return OperationResult.Exito();
        }

        // validaciones de notif
        public static OperationResult IsValidDto(this NotificationDto dto)
        {
            var errores = new List<string>();

            if (dto.RecipientId <= 0) errores.Add("El ID del destinatario es requerido.");
            if (string.IsNullOrWhiteSpace(dto.Title)) errores.Add("El título de la notificación es requerido.");
            if (string.IsNullOrWhiteSpace(dto.Message)) errores.Add("El mensaje de la notificación es requerido.");

            return errores.Count > 0 ? OperationResult.Fallo("Errores de validación de notificación.", errores) : OperationResult.Exito();
        }

        //validaciones medical record
        public static OperationResult IsValidDto(this CreateMedicalRecordDto dto)
        {
            var errores = new List<string>();

            if (dto.PatientId <= 0)
                errores.Add("El ID del paciente es requerido.");

            if (dto.DoctorId <= 0)
                errores.Add("El ID del doctor es requerido.");

            if (string.IsNullOrWhiteSpace(dto.Diagnosis)) errores.Add("El campo de diagnóstico es requerido.");
            if (string.IsNullOrWhiteSpace(dto.Treatment)) errores.Add("El campo de tratamiento es requerido.");

            return errores.Count > 0 ? OperationResult.Fallo("Errores de validación de registro médico.", errores) : OperationResult.Exito();
        }

        public static OperationResult IsValidDto(this UpdateMedicalRecordDto dto)
        {
            var errores = new List<string>();

            if (dto.RecordId <= 0) errores.Add("El ID del registro médico es inválido.");

            if (dto.PatientId is not null)
                errores.Add("El ID del paciente es requerido.");

            if (dto.DoctorId is not null)
                errores.Add("El ID del doctor es requerido.");

            if (string.IsNullOrWhiteSpace(dto.Diagnosis)) errores.Add("El campo de diagnóstico es requerido.");
            if (string.IsNullOrWhiteSpace(dto.Treatment)) errores.Add("El campo de tratamiento es requerido.");

            return errores.Count > 0 ? OperationResult.Fallo("Errores de validación de actualización de registro médico.", errores) : OperationResult.Exito();
        }

        // validaciones de disponibilidad
        public static OperationResult IsValidDto(this CreateAvailabilityDto dto)
        {
            var errores = new List<string>();

            if (dto.DoctorId <= 0) errores.Add("El ID del doctor es requerido.");

            if (dto.DayOfWeek < 0 || dto.DayOfWeek > 6) errores.Add("El día de la semana es inválido (debe ser 0-6).");

            if (dto.StartTime >= dto.EndTime) errores.Add("La hora de inicio debe ser menor que la hora de fin.");
            if (dto.StartTime.Hours < 7 || dto.EndTime.Hours > 19) errores.Add("El horario de disponibilidad debe estar entre 07:00 y 19:00.");

            return errores.Count > 0 ? OperationResult.Fallo("Errores de validación de disponibilidad.", errores) : OperationResult.Exito();
        }

        public static OperationResult IsValidDto(this UpdateAvailabilityDto dto)
        {
            var errores = new List<string>();

            if (dto.AvailabilityId <= 0) errores.Add("El ID de disponibilidad es inválido.");
            if (dto.DoctorId <= 0) errores.Add("El ID del doctor es requerido.");
            if (dto.DayOfWeek < 0 || dto.DayOfWeek > 6) errores.Add("El día de la semana es inválido (debe ser 0-6).");
            if (dto.StartTime >= dto.EndTime) errores.Add("La hora de inicio debe ser menor que la hora de fin.");
            if (dto.StartTime.Hours < 7 || dto.EndTime.Hours > 19) errores.Add("El horario de disponibilidad debe estar entre 07:00 y 19:00.");


            return errores.Count > 0 ? OperationResult.Fallo("Errores de validación de actualización de disponibilidad.", errores) : OperationResult.Exito();
        }
    }
}