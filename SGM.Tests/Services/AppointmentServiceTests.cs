using Moq;
using SGMC.Application.Dto.Appointments;
using SGMC.Application.Services;
using SGMC.Domain.Entities.Appointments;
using SGMC.Domain.Entities.Users;
using SGMC.Domain.Entities.System;
using SGMC.Domain.Repositories.Appointments;
using SGMC.Domain.Repositories.Users;
using SGMC.Domain.Repositories.System;
using Microsoft.Extensions.Logging;

namespace SGMC.Tests.Services
{
    public class AppointmentServiceTests
    {
        private readonly Mock<IAppointmentRepository> _appointmentRepoMock;
        private readonly Mock<IDoctorRepository> _doctorRepoMock;
        private readonly Mock<IPatientRepository> _patientRepoMock;
        private readonly Mock<IStatusRepository> _statusRepoMock;
        private readonly Mock<IDoctorAvailabilityRepository> _availabilityRepoMock;
        private readonly Mock<INotificationRepository> _notificationRepoMock;
        private readonly Mock<ILogger<AppointmentService>> _loggerMock;
        private readonly AppointmentService _service;

        public AppointmentServiceTests()
        {
            _appointmentRepoMock = new Mock<IAppointmentRepository>();
            _doctorRepoMock = new Mock<IDoctorRepository>();
            _patientRepoMock = new Mock<IPatientRepository>();
            _statusRepoMock = new Mock<IStatusRepository>();
            _availabilityRepoMock = new Mock<IDoctorAvailabilityRepository>();
            _notificationRepoMock = new Mock<INotificationRepository>();
            _loggerMock = new Mock<ILogger<AppointmentService>>();

            _service = new AppointmentService(
                _appointmentRepoMock.Object,
                _patientRepoMock.Object,
                _doctorRepoMock.Object,
                _loggerMock.Object
            );
        }
        private static DateTime GetValidDate()
        {
            var validDate = DateTime.Now.AddDays(2).Date.AddHours(14);

            if (validDate.DayOfWeek == DayOfWeek.Sunday)
            {
                validDate = validDate.AddDays(1);
            }

            return validDate;
        }

        [Fact]
        public async Task CreateAsync_WhenDtoIsNull_ReturnsFailure()
        {
            var result = await _service.CreateAsync(null!);
            Assert.False(result.Exitoso);
            Assert.Equal("Los datos de la cita son requeridos", result.Mensaje);
        }

        [Fact]
        public async Task CreateAsync_WhenPatientDoesNotExist_ReturnsFailure()
        {
            // ARRANGE
            var dto = new CreateAppointmentDto
            {
                PatientId = 999,
                DoctorId = 1,
                AppointmentDate = GetValidDate(),
            };

            _patientRepoMock.Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((Patient?)null);

            // ACT
            var result = await _service.CreateAsync(dto);

            // ASSERT
            Assert.False(result.Exitoso);
            Assert.Equal("El paciente no existe", result.Mensaje);
        }

        [Fact]
        public async Task CreateAsync_WhenValid_ReturnsSuccess()
        {
            // ARRANGE
            var validDate = GetValidDate();
            var dto = new CreateAppointmentDto
            {
                PatientId = 1,
                DoctorId = 1,
                AppointmentDate = validDate,
            };

            var patient = new Patient
            {
                PatientId = 1,
                IsActive = true,
                PhoneNumber = "123-456-7890",
                CreatedAt = DateTime.Now
            };

            var doctor = new Doctor
            {
                DoctorId = 1,
                IsActive = true,
                LicenseNumber = "LIC001",
                SpecialtyId = 1,
                CreatedAt = DateTime.Now
            };

            var appointmentResult = new Appointment
            {
                AppointmentId = 1,
                PatientId = 1,
                DoctorId = 1,
                AppointmentDate = dto.AppointmentDate,
                StatusId = 1,
                CreatedAt = DateTime.Now,
                Patient = patient,
                Doctor = doctor,
                Status = new Status { StatusId = 1, StatusName = "Pendiente" }
            };

            _patientRepoMock.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(patient);

            _doctorRepoMock.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(doctor);

            _appointmentRepoMock.Setup(r => r.ExistsInTimeSlotAsync(1, validDate))
                .ReturnsAsync(false);

            _appointmentRepoMock.Setup(r => r.GetByPatientIdAsync(1))
                .ReturnsAsync(new List<Appointment>());

            _appointmentRepoMock.Setup(r => r.AddAsync(It.IsAny<Appointment>()))
                .ReturnsAsync(appointmentResult);

            // ACT
            var result = await _service.CreateAsync(dto);

            // ASSERT
            Assert.True(result.Exitoso);
            Assert.Equal("Cita creada correctamente", result.Mensaje);
            Assert.NotNull(result.Datos);
        }
    }
}