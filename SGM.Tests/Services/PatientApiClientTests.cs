using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Moq;
using Moq.Protected;
using SGMC.Application.Dto.Users;
using SGMC.Web.Models;
using SGMC.Web.Services;
using Xunit;

namespace SGMC.Web.Tests.Services
{
    public class PatientApiClientTests
    {
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _httpClient;
        private readonly PatientApiClient _patientApiClient;

        public PatientApiClientTests()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://localhost:5038/api/")
            };
            _patientApiClient = new PatientApiClient(_httpClient);
        }

        [Fact]
        public async Task GetAllAsync_WhenApiReturnsSuccess_ShouldReturnPatientsList()
        {
            // Arrange
            var expectedPatients = new List<PatientDto>
            {
                new PatientDto
                {
                    PatientId = 1,
                    FirstName = "Juan",
                    LastName = "Pérez",
                    IdentificationNumber = "001-0000001-1",
                    PhoneNumber = "809-555-1234",
                    Email = "juan@test.com",
                    BloodType = "O+",
                    InsuranceProviderName = "ARS Universal",
                    IsActive = true
                },
                new PatientDto
                {
                    PatientId = 2,
                    FirstName = "María",
                    LastName = "García",
                    IdentificationNumber = "001-0000002-2",
                    PhoneNumber = "809-555-5678",
                    Email = "maria@test.com",
                    BloodType = "A+",
                    InsuranceProviderName = "Humano Seguros",
                    IsActive = true
                }
            };

            var operationResult = new OperationResultDto<List<PatientDto>>
            {
                Exitoso = true,
                Mensaje = "Pacientes obtenidos correctamente",
                Datos = expectedPatients
            };

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var responseContent = JsonSerializer.Serialize(operationResult, jsonOptions);

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri!.ToString().EndsWith("Patients")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
                });

            // Act
            var result = await _patientApiClient.GetAllAsync();

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Should().HaveCount(2);
            result.Data![0].FirstName.Should().Be("Juan");
            result.Data![0].LastName.Should().Be("Pérez");
            result.Data![1].FirstName.Should().Be("María");
            result.Data![1].LastName.Should().Be("García");
        }

        [Fact]
        public async Task GetByIdAsync_WhenPatientExists_ShouldReturnPatient()
        {
            // Arrange
            var expectedPatient = new PatientDto
            {
                PatientId = 1,
                FirstName = "Juan",
                LastName = "Pérez",
                IdentificationNumber = "001-0000001-1",
                Email = "juan.perez@example.com",
                PhoneNumber = "809-555-1234",
                Address = "Calle Principal #123, Santo Domingo",
                BloodType = "O+",
                Allergies = "Ninguna",
                InsuranceProviderId = 1,
                InsuranceProviderName = "ARS Universal",
                EmergencyContactName = "María Pérez",
                EmergencyContactPhone = "809-555-9999",
                IsActive = true
            };

            var operationResult = new OperationResultDto<PatientDto>
            {
                Exitoso = true,
                Mensaje = "Paciente obtenido correctamente",
                Datos = expectedPatient
            };

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var responseContent = JsonSerializer.Serialize(operationResult, jsonOptions);

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri!.ToString().EndsWith("Patients/1")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
                });

            // Act
            var result = await _patientApiClient.GetByIdAsync(1);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.PatientId.Should().Be(1);
            result.Data.FirstName.Should().Be("Juan");
            result.Data.LastName.Should().Be("Pérez");
            result.Data.BloodType.Should().Be("O+");
        }

        [Fact]
        public async Task CreateAsync_WithValidData_ShouldReturnCreatedPatient()
        {
            // Arrange
            var createDto = new RegisterPatientDto
            {
                FirstName = "Carlos",
                LastName = "Rodríguez",
                IdentificationNumber = "001-0000003-3",
                DateOfBirth = new DateOnly(1990, 5, 15),
                Gender = "M",
                Email = "carlos.rodriguez@example.com",
                Password = "Password123",
                PhoneNumber = "809-555-7777",
                Address = "Av. Winston Churchill, Santo Domingo",
                EmergencyContactName = "Ana Rodríguez",
                EmergencyContactPhone = "809-555-8888",
                BloodType = "B+",
                Allergies = "Penicilina",
                InsuranceProviderId = 2
            };

            var createdPatient = new PatientDto
            {
                PatientId = 3,
                FirstName = "Carlos",
                LastName = "Rodríguez",
                IdentificationNumber = "001-0000003-3",
                Email = "carlos.rodriguez@example.com",
                PhoneNumber = "809-555-7777",
                BloodType = "B+",
                InsuranceProviderName = "Humano Seguros",
                IsActive = true
            };

            var operationResult = new OperationResultDto<PatientDto>
            {
                Exitoso = true,
                Mensaje = "Paciente creado correctamente",
                Datos = createdPatient
            };

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var responseContent = JsonSerializer.Serialize(operationResult, jsonOptions);

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri!.ToString().EndsWith("Patients")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Created,
                    Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
                });

            // Act
            var result = await _patientApiClient.CreateAsync(createDto);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.PatientId.Should().Be(3);
            result.Data.FirstName.Should().Be("Carlos");
            result.Data.LastName.Should().Be("Rodríguez");
        }

        [Fact]
        public async Task CreateAsync_WithInvalidData_ShouldReturnFailure()
        {
            // Arrange
            var createDto = new RegisterPatientDto
            {
                FirstName = "Test",
                LastName = "Patient",
            };

            var operationResult = new OperationResultDto<PatientDto>
            {
                Exitoso = false,
                Mensaje = "Datos inválidos",
                Datos = null
            };

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var responseContent = JsonSerializer.Serialize(operationResult, jsonOptions);

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
                });

            // Act
            var result = await _patientApiClient.CreateAsync(createDto);

            // Assert
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task UpdateAsync_WithValidData_ShouldReturnUpdatedPatient()
        {
            // Arrange
            var updateDto = new UpdatePatientDto
            {
                PatientId = 1,
                PhoneNumber = "809-555-9999",
                Address = "Nueva Dirección 456",
                EmergencyContactName = "Pedro Pérez",
                EmergencyContactPhone = "809-555-0000",
                Allergies = "Ninguna conocida",
                InsuranceProviderId = 1
            };

            var updatedPatient = new PatientDto
            {
                PatientId = 1,
                FirstName = "Juan",
                LastName = "Pérez",
                PhoneNumber = "809-555-9999",
                Address = "Nueva Dirección 456",
                IsActive = true
            };

            var operationResult = new OperationResultDto<PatientDto>
            {
                Exitoso = true,
                Mensaje = "Paciente actualizado correctamente",
                Datos = updatedPatient
            };

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var responseContent = JsonSerializer.Serialize(operationResult, jsonOptions);

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Put &&
                        req.RequestUri!.ToString().EndsWith("Patients/1")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
                });

            // Act
            var result = await _patientApiClient.UpdateAsync(updateDto);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.PhoneNumber.Should().Be("809-555-9999");
        }

        [Fact]
        public async Task DeleteAsync_WhenPatientExists_ShouldReturnSuccess()
        {
            // Arrange
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Delete &&
                        req.RequestUri!.ToString().EndsWith("Patients/1")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                });

            // Act
            var result = await _patientApiClient.DeleteAsync(1);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteAsync_WhenPatientNotFound_ShouldReturnFailure()
        {
            // Arrange
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound,
                    ReasonPhrase = "Not Found"
                });

            // Act
            var result = await _patientApiClient.DeleteAsync(999);

            // Assert
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Contain("404");
        }
    }
}