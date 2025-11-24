using SGMC.Application.Dto.Insurance;
using SGMC.Domain.Base;

namespace SGMC.Application.Validators.Insurance
{
    // Validador para DTOs de InsuranceProvider
    public static class InsuranceProviderValidator
    {
        // Valida CreateInsuranceProviderDto
        public static OperationResult IsValidDto(this CreateInsuranceProviderDto dto)
        {
            var errores = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.Name))
                errores.Add("El nombre es requerido.");

            if (string.IsNullOrWhiteSpace(dto.PhoneNumber))
                errores.Add("El teléfono es requerido.");

            if (string.IsNullOrWhiteSpace(dto.Email))
                errores.Add("Email inválido o requerido.");

            if (string.IsNullOrWhiteSpace(dto.Address))
                errores.Add("La dirección es requerida.");

            if (dto.NetworkTypeId <= 0)
                errores.Add("El tipo de red es requerido.");

            return errores.Count > 0
                ? OperationResult.Fallo("Errores de validación de proveedor de seguro.", errores)
                : OperationResult.Exito();
        }

        // Valida UpdateInsuranceProviderDto
        public static OperationResult IsValidDto(this UpdateInsuranceProviderDto dto)
        {
            var errores = new List<string>();

            if (dto.InsuranceProviderId <= 0)
                errores.Add("El ID del proveedor es inválido.");

            if (dto.NetworkTypeId <= 0)
                errores.Add("El tipo de red es requerido.");

            return errores.Count > 0
                ? OperationResult.Fallo("Errores de validación de actualización de proveedor de seguro.", errores)
                : OperationResult.Exito();
        }
    }
}