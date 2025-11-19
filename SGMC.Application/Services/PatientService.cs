using Microsoft.Extensions.Logging;
using SGMC.Application.Dto.Users;
using SGMC.Application.Extensions;
using SGMC.Application.Interfaces.Service;
using SGMC.Domain.Base;
using SGMC.Domain.Entities.Users;
using SGMC.Domain.Repositories.Insurance;
using SGMC.Domain.Repositories.Users;

namespace SGMC.Application.Services
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _repository;
        private readonly ILogger<PatientService> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IPersonRepository _personRepository;
        private readonly IInsuranceProviderRepository _insuranceProviderRepository;

        public PatientService(
            IPatientRepository repository,
            ILogger<PatientService> logger,
            IUserRepository userRepository,
            IPersonRepository personRepository,
            IInsuranceProviderRepository insuranceProviderRepository
        )
        {
            _repository = repository;
            _logger = logger;
            _userRepository = userRepository;
            _personRepository = personRepository;
            _insuranceProviderRepository = insuranceProviderRepository;
        }

        public async Task<OperationResult<PatientDto>> CreateAsync(RegisterPatientDto dto)
        {
            // validaciones de campo fuera de trycatch
            if (dto == null)
                return OperationResult<PatientDto>.Fallo("Los datos del paciente son requeridos");

            // extension method para validacion de campos
            var validationResult = dto.IsValidDto();
            if (!validationResult.Exitoso)
                // Retornar los errores de validacion de campos
                return OperationResult<PatientDto>.Fallo(validationResult.Mensaje, validationResult.Errores);

            try
            {
                // validaciones de negocio que requieren acceso a bd

                if (await _personRepository.ExistsByIdentificationNumberAsync(dto.IdentificationNumber))
                    return OperationResult<PatientDto>.Fallo("Ya existe una persona con esa cédula");

                if (await _userRepository.ExistsByEmailAsync(dto.Email))
                    return OperationResult<PatientDto>.Fallo("El email ya está en uso");

                var insuranceExists = await _insuranceProviderRepository.ExistsAsync(dto.InsuranceProviderId);
                if (!insuranceExists)
                    return OperationResult<PatientDto>.Fallo("El proveedor de seguro seleccionado no existe");

                // create entities and persistence

                // create user
                var user = new User
                {
                    Email = dto.Email.ToLower().Trim(),
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    RoleId = 3, // patient
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                var createdUserResult = await _userRepository.AddAsync(user);
                if (!createdUserResult.Exitoso)
                    return OperationResult<PatientDto>.Fallo(createdUserResult.Mensaje ?? "No se pudo crear el usuario");

                if (createdUserResult.Datos is not User createdUser)
                    return OperationResult<PatientDto>.Fallo("El repositorio no devolvió un User válido");

                int newId = createdUser.UserId;

                // create person
                var person = new Person
                {
                    PersonId = newId,
                    FirstName = dto.FirstName.Trim(),
                    LastName = dto.LastName.Trim(),
                    DateOfBirth = dto.DateOfBirth,
                    IdentificationNumber = dto.IdentificationNumber.Trim(),
                    Gender = dto.Gender
                };
                createdUser.UserNavigation = person;
                await _personRepository.AddAsync(person);

                // create patient
                var patient = new Patient
                {
                    PatientId = newId,
                    Gender = dto.Gender,
                    PhoneNumber = dto.PhoneNumber.Trim(),
                    Address = dto.Address.Trim(),
                    EmergencyContactName = dto.EmergencyContactName.Trim(),
                    EmergencyContactPhone = dto.EmergencyContactPhone.Trim(),
                    BloodType = dto.BloodType,
                    Allergies = dto.Allergies?.Trim() ?? string.Empty,
                    InsuranceProviderId = dto.InsuranceProviderId,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    PatientNavigation = person
                };

                var createdPatient = await _repository.AddAsync(patient);
                if (createdPatient == null)
                    return OperationResult<PatientDto>.Fallo("No se pudo crear el paciente");

                var dtoResult = MapToDto(createdPatient);
                dtoResult.Email = createdUser.Email;

                return OperationResult<PatientDto>.Exito(dtoResult, "Paciente creado correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear paciente");
                return OperationResult<PatientDto>.Fallo($"Error al crear paciente: {ex.Message}");
            }
        }

        public async Task<OperationResult<PatientDto>> UpdateAsync(UpdatePatientDto dto)
        {
            // 1. Validaciones de campo fuera del try-catch
            if (dto == null)
                return OperationResult<PatientDto>.Fallo("Los datos del paciente son requeridos");

            // extension method para validacion de campos
            var validationResult = dto.IsValidDto();
            if (!validationResult.Exitoso)
                return OperationResult<PatientDto>.Fallo(validationResult.Mensaje, validationResult.Errores);

            try
            {
                // validaciones de negocio
                var patient = await _repository.GetByIdWithDetailsAsync(dto.PatientId);
                if (patient == null)
                    return OperationResult<PatientDto>.Fallo("Paciente no encontrado");

                var insuranceExists = await _insuranceProviderRepository.ExistsAsync(dto.InsuranceProviderId);
                if (!insuranceExists)
                    return OperationResult<PatientDto>.Fallo("El proveedor de seguro seleccionado no existe");

                // update entity
                patient.PhoneNumber = dto.PhoneNumber.Trim();
                patient.Address = dto.Address.Trim();
                patient.EmergencyContactName = dto.EmergencyContactName.Trim();
                patient.EmergencyContactPhone = dto.EmergencyContactPhone.Trim();
                patient.Allergies = dto.Allergies?.Trim() ?? string.Empty;
                patient.InsuranceProviderId = dto.InsuranceProviderId;
                patient.UpdatedAt = DateTime.Now;

                await _repository.UpdateAsync(patient);

                return OperationResult<PatientDto>.Exito(
                    MapToDto(patient),
                    "Paciente actualizado correctamente"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar paciente {Id}", dto?.PatientId);
                return OperationResult<PatientDto>.Fallo($"Error al actualizar paciente: {ex.Message}");
            }
        }

        public async Task<OperationResult> DeleteAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return OperationResult.Fallo("El ID del paciente es inválido");

                var patient = await _repository.GetByIdAsync(id);
                if (patient == null)
                    return OperationResult.Fallo("Paciente no encontrado");

                var user = await _userRepository.GetByIdAsync(id);
                if (user != null)
                {
                    user.IsActive = false;
                    await _userRepository.UpdateAsync(user);
                }

                patient.IsActive = false;
                patient.UpdatedAt = DateTime.Now;

                await _repository.UpdateAsync(patient);

                return OperationResult.Exito("Paciente desactivado correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar paciente {Id}", id);
                return OperationResult.Fallo($"Error al eliminar paciente: {ex.Message}");
            }
        }

        public async Task<OperationResult<List<PatientDto>>> GetAllAsync()
        {
            try
            {
                var patients = await _repository.GetAllWithDetailsAsync();
                return OperationResult<List<PatientDto>>.Exito(
                    patients.Select(MapToDto).ToList(),
                    "Pacientes obtenidos correctamente"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener pacientes");
                return OperationResult<List<PatientDto>>.Fallo("Error al obtener pacientes");
            }
        }

        public async Task<OperationResult<List<PatientDto>>> GetActiveAsync()
        {
            try
            {
                var patients = await _repository.GetActivePatientsAsync();
                return OperationResult<List<PatientDto>>.Exito(
                    patients.Select(MapToDto).ToList(),
                    "Pacientes activos obtenidos correctamente"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener pacientes activos");
                return OperationResult<List<PatientDto>>.Fallo("Error al obtener pacientes activos");
            }
        }

        public async Task<OperationResult<List<PatientDto>>> GetByInsuranceProviderAsync(int insuranceProviderId)
        {
            try
            {
                if (insuranceProviderId <= 0)
                    return OperationResult<List<PatientDto>>.Fallo("El ID del proveedor de seguro es inválido");

                var patients = await _repository.GetByInsuranceProviderIdAsync(insuranceProviderId);
                return OperationResult<List<PatientDto>>.Exito(
                    patients.Select(MapToDto).ToList(),
                    "Pacientes obtenidos por proveedor de seguro"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener pacientes por seguro {Id}", insuranceProviderId);
                return OperationResult<List<PatientDto>>.Fallo("Error al obtener pacientes");
            }
        }

        public async Task<OperationResult<PatientDto>> GetByPhoneNumberAsync(string phoneNumber)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(phoneNumber))
                    return OperationResult<PatientDto>.Fallo("El número de teléfono es requerido");

                var patient = await _repository.GetByPhoneNumberAsync(phoneNumber);
                if (patient == null)
                    return OperationResult<PatientDto>.Fallo("Paciente no encontrado");

                return OperationResult<PatientDto>.Exito(
                    MapToDto(patient),
                    "Paciente obtenido correctamente"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener paciente por teléfono {Phone}", phoneNumber);
                return OperationResult<PatientDto>.Fallo("Error al obtener paciente");
            }
        }

        public async Task<OperationResult<bool>> ExistsAsync(int patientId)
        {
            try
            {
                if (patientId <= 0)
                    return OperationResult<bool>.Fallo("El ID del paciente es inválido");

                var exists = await _repository.ExistsAsync(patientId);
                return OperationResult<bool>.Exito(exists, "Verificación completada");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar paciente {Id}", patientId);
                return OperationResult<bool>.Fallo("Error al verificar paciente");
            }
        }

        public async Task<OperationResult<PatientDto>> GetByIdWithDetailsAsync(int patientId)
        {
            try
            {
                if (patientId <= 0)
                    return OperationResult<PatientDto>.Fallo("El ID del paciente es inválido");

                var patient = await _repository.GetByIdWithDetailsAsync(patientId);
                if (patient == null)
                    return OperationResult<PatientDto>.Fallo("Paciente no encontrado");

                return OperationResult<PatientDto>.Exito(
                    MapToDto(patient),
                    "Paciente con detalles obtenido correctamente"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener paciente con detalles {Id}", patientId);
                return OperationResult<PatientDto>.Fallo("Error al obtener paciente con detalles");
            }
        }

        public async Task<OperationResult<List<PatientDto>>> GetWithAppointmentsAsync(int patientId)
        {
            try
            {
                if (patientId <= 0)
                    return OperationResult<List<PatientDto>>.Fallo("El ID del paciente es inválido");

                var patient = await _repository.GetByIdWithAppointmentsAsync(patientId);
                if (patient == null)
                    return OperationResult<List<PatientDto>>.Fallo("Paciente no encontrado");

                return OperationResult<List<PatientDto>>.Exito(
                    new List<PatientDto> { MapToDto(patient) },
                    "Paciente con citas obtenido correctamente"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener paciente con citas {Id}", patientId);
                return OperationResult<List<PatientDto>>.Fallo("Error al obtener paciente con citas");
            }
        }

        public async Task<OperationResult<List<PatientDto>>> GetWithMedicalRecordsAsync(int patientId)
        {
            try
            {
                if (patientId <= 0)
                    return OperationResult<List<PatientDto>>.Fallo("El ID del paciente es inválido");

                var patient = await _repository.GetByIdWithMedicalRecordsAsync(patientId);
                if (patient == null)
                    return OperationResult<List<PatientDto>>.Fallo("Paciente no encontrado");

                return OperationResult<List<PatientDto>>.Exito(
                    new List<PatientDto> { MapToDto(patient) },
                    "Paciente con registros médicos obtenido correctamente"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener paciente con registros {Id}", patientId);
                return OperationResult<List<PatientDto>>.Fallo("Error al obtener paciente con registros médicos");
            }
        }

        public async Task<OperationResult<PatientDto>> GetByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return OperationResult<PatientDto>.Fallo("El ID del paciente es inválido");

                var patient = await _repository.GetByIdWithDetailsAsync(id);
                if (patient == null)
                    return OperationResult<PatientDto>.Fallo("Paciente no encontrado");

                return OperationResult<PatientDto>.Exito(
                    MapToDto(patient),
                    "Paciente obtenido correctamente"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener paciente {Id}", id);
                return OperationResult<PatientDto>.Fallo("Error al obtener paciente");
            }
        }

        private static PatientDto MapToDto(Patient p) => new()
        {
            PatientId = p.PatientId,
            FirstName = p.PatientNavigation?.FirstName ?? string.Empty,
            LastName = p.PatientNavigation?.LastName ?? string.Empty,
            DateOfBirth = p.PatientNavigation?.DateOfBirth,
            IdentificationNumber = p.PatientNavigation?.IdentificationNumber ?? string.Empty,
            Email = p.PatientNavigation?.User?.Email ?? string.Empty,
            Gender = p.Gender,
            PhoneNumber = p.PhoneNumber,
            Address = p.Address,
            EmergencyContactName = p.EmergencyContactName,
            EmergencyContactPhone = p.EmergencyContactPhone,
            BloodType = p.BloodType,
            Allergies = p.Allergies,
            InsuranceProviderId = p.InsuranceProviderId,
            InsuranceProviderName = p.InsuranceProvider?.Name ?? string.Empty,
            IsActive = p.IsActive
        };
    }
}
