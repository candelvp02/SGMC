using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Moq;
using Moq.Protected;
using SGMC.Application.Dto.Appointments;
using SGMC.Web.Models;
using SGMC.Web.Services;
using Xunit;

namespace SGMC.Web.Tests.Services
{
    public class AppointmentApiClientTests
    {
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _httpClient;
        private readonly AppointmentApiClient _appointmentApiClient;

        public AppointmentApiClientTests()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://localhost:5038/api/")
            };
            _appointmentApiClient = new AppointmentApiClient(_httpClient);
        }

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_WhenApiReturnsSuccess_ShouldReturnAppointmentsList()
        {
            // Arrange
            var expectedAppointments = new List<AppointmentDto>
            {
                new AppointmentDto
                {
                    AppointmentId = 1,
                    PatientId = 1,
                    PatientName = "Juan Pérez",
                    DoctorId = 1,
                    DoctorName = "Dr. Carlos Fernández",
                    AppointmentDate = DateTime.Now.AddDays(1),
                    StatusId = 1,
                    StatusName = "Pendiente",
                    CreatedAt = DateTime.Now
                },
                new AppointmentDto
                {
                    AppointmentId = 2,
                    PatientId = 2,
                    PatientName = "María García",
                    DoctorId = 2,
                    DoctorName = "Dra. Ana Martínez",
                    AppointmentDate = DateTime.Now.AddDays(2),
                    StatusId = 2,
                    StatusName = "Confirmada",
                    CreatedAt = DateTime.Now
                }
            };

            var operationResult = new OperationResultDto<List<AppointmentDto>>
            {
                Exitoso = true,
                Mensaje = "Citas obtenidas correctamente",
                Datos = expectedAppointments
            };

            var responseContent = JsonSerializer.Serialize(operationResult);

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri!.ToString().EndsWith("Appointments")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
                });

            // Act
            var result = await _appointmentApiClient.GetAllAsync();

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Should().HaveCount(2);
            result.Data![0].PatientName.Should().Be("Juan Pérez");
            result.Data![1].StatusName.Should().Be("Confirmada");
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_WhenAppointmentExists_ShouldReturnAppointment()
        {
            // Arrange
            var expectedAppointment = new AppointmentDto
            {
                AppointmentId = 1,
                PatientId = 1,
                PatientName = "Juan Pérez",
                DoctorId = 1,
                DoctorName = "Dr. Carlos Fernández",
                AppointmentDate = new DateTime(2025, 12, 1, 10, 0, 0),
                StatusId = 2,
                StatusName = "Confirmada",
                CreatedAt = DateTime.Now.AddDays(-5)
            };

            var operationResult = new OperationResultDto<AppointmentDto>
            {
                Exitoso = true,
                Mensaje = "Cita obtenida correctamente",
                Datos = expectedAppointment
            };

            var responseContent = JsonSerializer.Serialize(operationResult);

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri!.ToString().EndsWith("Appointments/1")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
                });

            // Act
            var result = await _appointmentApiClient.GetByIdAsync(1);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.AppointmentId.Should().Be(1);
            result.Data.StatusName.Should().Be("Confirmada");
        }

        #endregion

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_WithValidData_ShouldReturnCreatedAppointment()
        {
            // Arrange
            var createDto = new CreateAppointmentDto
            {
                PatientId = 1,
                DoctorId = 1,
                AppointmentDate = DateTime.Now.AddDays(3).Date.AddHours(14)
            };

            var createdAppointment = new AppointmentDto
            {
                AppointmentId = 3,
                PatientId = 1,
                PatientName = "Juan Pérez",
                DoctorId = 1,
                DoctorName = "Dr. Carlos Fernández",
                AppointmentDate = createDto.AppointmentDate,
                StatusId = 1,
                StatusName = "Pendiente",
                CreatedAt = DateTime.Now
            };

            var operationResult = new OperationResultDto<AppointmentDto>
            {
                Exitoso = true,
                Mensaje = "Cita creada correctamente",
                Datos = createdAppointment
            };

            var responseContent = JsonSerializer.Serialize(operationResult);

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri!.ToString().EndsWith("Appointments")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Created,
                    Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
                });

            // Act
            var result = await _appointmentApiClient.CreateAsync(createDto);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.AppointmentId.Should().Be(3);
            result.Data.StatusName.Should().Be("Pendiente");
        }

        [Fact]
        public async Task CreateAsync_WithConflictingSchedule_ShouldReturnFailure()
        {
            // Arrange
            var createDto = new CreateAppointmentDto
            {
                PatientId = 1,
                DoctorId = 1,
                AppointmentDate = DateTime.Now.AddDays(1).Date.AddHours(10)
            };

            var operationResult = new OperationResultDto<AppointmentDto>
            {
                Exitoso = false,
                Mensaje = "El doctor ya tiene una cita en ese horario",
                Datos = null
            };

            var responseContent = JsonSerializer.Serialize(operationResult);

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
            var result = await _appointmentApiClient.CreateAsync(createDto);

            // Assert
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().NotBeNullOrEmpty();
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_WithValidData_ShouldReturnUpdatedAppointment()
        {
            // Arrange
            var updateDto = new UpdateAppointmentDto
            {
                Id = 1,
                AppointmentId = 1,
                AppointmentDate = DateTime.Now.AddDays(5).Date.AddHours(15),
                StatusId = 2,
                Notes = "Confirmada por el paciente"
            };

            var updatedAppointment = new AppointmentDto
            {
                AppointmentId = 1,
                AppointmentDate = updateDto.AppointmentDate,
                StatusId = 2,
                StatusName = "Confirmada"
            };

            var operationResult = new OperationResultDto<AppointmentDto>
            {
                Exitoso = true,
                Mensaje = "Cita actualizada correctamente",
                Datos = updatedAppointment
            };

            var responseContent = JsonSerializer.Serialize(operationResult);

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Put &&
                        req.RequestUri!.ToString().EndsWith("Appointments/1")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
                });

            // Act
            var result = await _appointmentApiClient.UpdateAsync(updateDto);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.StatusName.Should().Be("Confirmada");
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_WhenAppointmentExists_ShouldReturnSuccess()
        {
            // Arrange
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Delete &&
                        req.RequestUri!.ToString().EndsWith("Appointments/1")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                });

            // Act
            var result = await _appointmentApiClient.DeleteAsync(1);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().BeTrue();
        }

        #endregion

        #region GetByPatientAsync Tests

        [Fact]
        public async Task GetByPatientAsync_WhenAppointmentsExist_ShouldReturnList()
        {
            // Arrange
            var expectedAppointments = new List<AppointmentDto>
            {
                new AppointmentDto
                {
                    AppointmentId = 1,
                    PatientId = 1,
                    PatientName = "Juan Pérez",
                    DoctorName = "Dr. Carlos Fernández",
                    AppointmentDate = DateTime.Now.AddDays(1),
                    StatusName = "Pendiente"
                }
            };

            var operationResult = new OperationResultDto<List<AppointmentDto>>
            {
                Exitoso = true,
                Mensaje = "Citas obtenidas correctamente",
                Datos = expectedAppointments
            };

            var responseContent = JsonSerializer.Serialize(operationResult);

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri!.ToString().Contains("patient/1")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
                });

            // Act
            var result = await _appointmentApiClient.GetByPatientAsync(1);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Should().HaveCount(1);
            result.Data![0].PatientId.Should().Be(1);
        }

        #endregion

        #region GetByDoctorAsync Tests

        [Fact]
        public async Task GetByDoctorAsync_WhenAppointmentsExist_ShouldReturnList()
        {
            // Arrange
            var expectedAppointments = new List<AppointmentDto>
            {
                new AppointmentDto
                {
                    AppointmentId = 2,
                    DoctorId = 1,
                    DoctorName = "Dr. Carlos Fernández",
                    PatientName = "María García",
                    AppointmentDate = DateTime.Now.AddDays(2),
                    StatusName = "Confirmada"
                }
            };

            var operationResult = new OperationResultDto<List<AppointmentDto>>
            {
                Exitoso = true,
                Mensaje = "Citas obtenidas correctamente",
                Datos = expectedAppointments
            };

            var responseContent = JsonSerializer.Serialize(operationResult);

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri!.ToString().Contains("doctor/1")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
                });

            // Act
            var result = await _appointmentApiClient.GetByDoctorAsync(1);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Should().HaveCount(1);
            result.Data![0].DoctorId.Should().Be(1);
        }

        #endregion
    }
}