using SGMC.Application.Dto.Medical;
using SGMC.Domain.Base;

namespace SGMC.Application.Validators.Medical
{
    // Validador para DTOs de MedicalRecord
    public static class MedicalRecordValidator
    {
        // Valida CreateMedicalRecordDto
        public static OperationResult IsValidDto(this CreateMedicalRecordDto dto)
        {
            var errores = new List<string>();

            if (dto.PatientId <= 0)
                errores.Add("El ID del paciente es requerido.");

            if (dto.DoctorId <= 0)
                errores.Add("El ID del doctor es requerido.");

            if (string.IsNullOrWhiteSpace(dto.Diagnosis))
                errores.Add("El campo de diagnóstico es requerido.");

            if (string.IsNullOrWhiteSpace(dto.Treatment))
                errores.Add("El campo de tratamiento es requerido.");

            return errores.Count > 0
                ? OperationResult.Fallo("Errores de validación de registro médico.", errores)
                : OperationResult.Exito();
        }

        // Valida UpdateMedicalRecordDto
        public static OperationResult IsValidDto(this UpdateMedicalRecordDto dto)
        {
            var errores = new List<string>();

            if (dto.RecordId <= 0)
                errores.Add("El ID del registro médico es inválido.");

            if (dto.PatientId is not null)
                errores.Add("El ID del paciente es requerido.");

            if (dto.DoctorId is not null)
                errores.Add("El ID del doctor es requerido.");

            if (string.IsNullOrWhiteSpace(dto.Diagnosis))
                errores.Add("El campo de diagnóstico es requerido.");

            if (string.IsNullOrWhiteSpace(dto.Treatment))
                errores.Add("El campo de tratamiento es requerido.");

            return errores.Count > 0
                ? OperationResult.Fallo("Errores de validación de actualización de registro médico.", errores)
                : OperationResult.Exito();
        }
    }
}