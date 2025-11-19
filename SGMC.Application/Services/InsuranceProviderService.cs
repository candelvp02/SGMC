using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGMC.Application.Dto.Insurance;
using SGMC.Application.Extensions;
using SGMC.Application.Interfaces.Service;
using SGMC.Domain.Base;
using SGMC.Domain.Entities.Insurance;
using SGMC.Domain.Repositories.Insurance;

namespace SGMC.Application.Services
{
    public class InsuranceProviderService : IInsuranceProviderService
    {
        private readonly IInsuranceProviderRepository _repository;
        private readonly INetworkTypeRepository _networkTypeRepository;
        private readonly ILogger<InsuranceProviderService> _logger;

        public InsuranceProviderService(
            IInsuranceProviderRepository repository,
            INetworkTypeRepository networkTypeRepository,
            ILogger<InsuranceProviderService> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _networkTypeRepository = networkTypeRepository ?? throw new ArgumentNullException(nameof(networkTypeRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OperationResult<InsuranceProviderDto>> CreateAsync(CreateInsuranceProviderDto dto)
        {
            // validaciones de campo fuera del trycatch
            if (dto == null)
                return OperationResult<InsuranceProviderDto>.Fallo("Los datos del proveedor son requeridos");

            var validationResult = dto.IsValidDto();
            if (!validationResult.Exitoso)
                return OperationResult<InsuranceProviderDto>.Fallo(validationResult.Mensaje, validationResult.Errores);

            try
            {
                // validaciones de negocio
                var networkTypeExists = await _networkTypeRepository.ExistsAsync(dto.NetworkTypeId);
                if (!networkTypeExists)
                    return OperationResult<InsuranceProviderDto>.Fallo("El tipo de red seleccionado no existe");

                _logger.LogInformation("Creando proveedor: {Name}", dto.Name);

                // create entity and save
                var provider = new InsuranceProvider
                {
                    Name = dto.Name.Trim(),
                    PhoneNumber = dto.PhoneNumber.Trim(),
                    Email = dto.Email.Trim().ToLower(),
                    Address = dto.Address.Trim(),
                    Website = dto.Website?.Trim() ?? string.Empty,
                    NetworkTypeId = dto.NetworkTypeId,
                    CoverageDetails = dto.CoverageDetails?.Trim() ?? "Cobertura estándar",
                    IsActive = true,
                    IsPreferred = false,
                    CreatedAt = DateTime.UtcNow
                };

                InsuranceProvider? created = await _repository.AddAsync(provider);
                if (created is null)
                    return OperationResult<InsuranceProviderDto>.Fallo("No se pudo crear el proveedor de seguro");

                return OperationResult<InsuranceProviderDto>.Exito(
                    MapToDto(created),
                    "Proveedor de seguro creado correctamente"
                );
            }
            catch (DbUpdateException dbEx)
            {
                var innerMessage = dbEx.InnerException?.Message ?? "Sin detalles adicionales";
                _logger.LogError(dbEx, "Error de BD al crear proveedor");
                return OperationResult<InsuranceProviderDto>.Fallo($"Error de base de datos: {innerMessage}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear proveedor");
                return OperationResult<InsuranceProviderDto>.Fallo($"Error al crear proveedor: {ex.Message}");
            }
        }

        public async Task<OperationResult<InsuranceProviderDto>> UpdateAsync(UpdateInsuranceProviderDto dto)
        {
            // validaciones de campo fuera del trycatch
            if (dto == null)
                return OperationResult<InsuranceProviderDto>.Fallo("Los datos del proveedor son requeridos");

            var validationResult = dto.IsValidDto();
            if (!validationResult.Exitoso)
                return OperationResult<InsuranceProviderDto>.Fallo(validationResult.Mensaje, validationResult.Errores);

            try
            {
                // validaciones de negocio
                var existing = await _repository.GetByIdAsync(dto.InsuranceProviderId);
                if (existing == null)
                    return OperationResult<InsuranceProviderDto>.Fallo("Proveedor de seguro no encontrado");

                var networkTypeExists = await _networkTypeRepository.ExistsAsync(dto.NetworkTypeId);
                if (!networkTypeExists)
                    return OperationResult<InsuranceProviderDto>.Fallo("El tipo de red seleccionado no existe");

                // update entity
                existing.Name = dto.Name.Trim();
                existing.PhoneNumber = dto.PhoneNumber?.Trim() ?? existing.PhoneNumber;
                existing.Email = dto.Email?.Trim().ToLower() ?? existing.Email;
                existing.Address = dto.Address?.Trim() ?? existing.Address;
                existing.Website = dto.Website?.Trim() ?? existing.Website;
                existing.NetworkTypeId = dto.NetworkTypeId;
                existing.IsActive = dto.IsActive;
                existing.UpdatedAt = DateTime.UtcNow;

                await _repository.UpdateAsync(existing);

                return OperationResult<InsuranceProviderDto>.Exito(
                    MapToDto(existing),
                    "Proveedor de seguro actualizado correctamente"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar proveedor {Id}", dto?.InsuranceProviderId);
                return OperationResult<InsuranceProviderDto>.Fallo($"Error al actualizar proveedor: {ex.Message}");
            }
        }
        public async Task<OperationResult> DeleteAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return OperationResult.Fallo("El ID del proveedor es inválido");

                var existing = await _repository.GetByIdAsync(id);
                if (existing == null)
                    return OperationResult.Fallo("Proveedor de seguro no encontrado");

                await _repository.DeleteAsync(id);
                return OperationResult.Exito("Proveedor de seguro eliminado correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar proveedor {Id}", id);
                return OperationResult.Fallo("Error al eliminar proveedor de seguro");
            }
        }

        public async Task<OperationResult<InsuranceProviderDto>> GetByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return OperationResult<InsuranceProviderDto>.Fallo("El ID del proveedor es inválido");

                var provider = await _repository.GetByIdAsync(id);
                if (provider == null)
                    return OperationResult<InsuranceProviderDto>.Fallo("Proveedor de seguro no encontrado");

                return OperationResult<InsuranceProviderDto>.Exito(
                    MapToDto(provider),
                    "Proveedor de seguro obtenido correctamente"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener proveedor {Id}", id);
                return OperationResult<InsuranceProviderDto>.Fallo("Error al obtener proveedor");
            }
        }

        public async Task<OperationResult<List<InsuranceProviderDto>>> GetActiveAsync()
        {
            try
            {
                var activeProviders = await _repository.GetActiveProviderAsync();
                return OperationResult<List<InsuranceProviderDto>>.Exito(
                    activeProviders.Select(MapToDto).ToList(),
                    "Proveedores activos obtenidos correctamente"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener proveedores activos");
                return OperationResult<List<InsuranceProviderDto>>.Fallo("Error al obtener proveedores activos");
            }
        }

        public async Task<OperationResult<bool>> ExistsAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return OperationResult<bool>.Exito(false, "ID inválido");

                var exists = await _repository.ExistsAsync(id);
                return OperationResult<bool>.Exito(
                    exists,
                    exists ? "El proveedor existe" : "El proveedor no existe"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar existencia de proveedor {Id}", id);
                return OperationResult<bool>.Fallo("Error al verificar proveedor");
            }
        }

        public async Task<OperationResult<List<InsuranceProviderDto>>> GetAllAsync()
        {
            try
            {
                var providers = await _repository.GetAllAsync();
                return OperationResult<List<InsuranceProviderDto>>.Exito(
                    providers.Select(MapToDto).ToList(),
                    "Proveedores obtenidos correctamente"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener proveedores");
                return OperationResult<List<InsuranceProviderDto>>.Fallo("Error al obtener proveedores");
            }
        }

        private static InsuranceProviderDto MapToDto(InsuranceProvider provider) => new()
        {
            InsuranceProviderId = provider.InsuranceProviderId,
            Name = provider.Name,
            PhoneNumber = provider.PhoneNumber,
            Email = provider.Email,
            Website = provider.Website,
            Address = provider.Address,
            City = provider.City,
            State = provider.State,
            Country = provider.Country,
            ZipCode = provider.ZipCode,
            CoverageDetails = provider.CoverageDetails ?? string.Empty,
            LogoUrl = provider.LogoUrl,
            IsPreferred = provider.IsPreferred,
            NetworkTypeId = provider.NetworkTypeId,
            NetworkTypeName = provider.NetworkType?.Name ?? string.Empty,
            MaxCoverageAmount = provider.MaxCoverageAmount,
            IsActive = provider.IsActive
        };
    }
}