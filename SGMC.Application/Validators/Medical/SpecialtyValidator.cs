using SGMC.Application.Dto.Medical;
using SGMC.Domain.Base;

namespace SGMC.Application.Validators.Medical
{
    // Validador para DTOs de Specialty
    public static class SpecialtyValidator
    {
        // Valida CreateSpecialtyDto
        public static OperationResult IsValidDto(this CreateSpecialtyDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.SpecialtyName))
                return OperationResult.Fallo("El nombre de la especialidad es requerido.");

            return OperationResult.Exito();
        }

        // Valida UpdateSpecialtyDto
        public static OperationResult IsValidDto(this UpdateSpecialtyDto dto)
        {
            var errores = new List<string>();

            if (dto.SpecialtyId <= 0)
                errores.Add("El ID de la especialidad es inválido.");

            if (string.IsNullOrWhiteSpace(dto.SpecialtyName))
                errores.Add("El nombre es requerido.");

            return errores.Count > 0
                ? OperationResult.Fallo("Errores de validación de especialidad.", errores)
                : OperationResult.Exito();
        }
    }
}