using Moq;
using SGMC.Application.Dto.Appointments;
using SGMC.Application.Services;
using SGMC.Domain.Entities.Appointments;
using SGMC.Domain.Entities.Users;
using SGMC.Domain.Entities.System;
using SGMC.Domain.Entities.Medical;
using SGMC.Domain.Repositories.Appointments;
using SGMC.Domain.Repositories.Users;
using Microsoft.Extensions.Logging;

namespace SGMC.Tests.Services
{
    public class AppointmentServiceTests
    {
        private readonly Mock<IAppointmentRepository> _appointmentRepoMock;
        private readonly Mock<IDoctorRepository> _doctorRepoMock;
        private readonly Mock<IPatientRepository> _patientRepoMock;
        private readonly Mock<ILogger<AppointmentService>> _loggerMock;
        private readonly AppointmentService _service;

        public AppointmentServiceTests()
        {
            _appointmentRepoMock = new Mock<IAppointmentRepository>();
            _doctorRepoMock = new Mock<IDoctorRepository>();
            _patientRepoMock = new Mock<IPatientRepository>();
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
            // Aseguramos que sea un día laborable y en horario válido
            var validDate = DateTime.Now.AddDays(3).Date.AddHours(10); // 10:00 AM

            // Si cae en domingo, movemos al lunes
            if (validDate.DayOfWeek == DayOfWeek.Sunday)
            {
                validDate = validDate.AddDays(1);
            }
            // Si cae en sábado, movemos al lunes
            else if (validDate.DayOfWeek == DayOfWeek.Saturday)
            {
                validDate = validDate.AddDays(2);
            }

            return validDate;
        }

        [Fact]
        public async Task CreateAsync_WhenDtoIsNull_ReturnsFailure()
        {
            // ACT
            var result = await _service.CreateAsync(null!);

            // ASSERT
            Assert.False(result.Exitoso);
            Assert.Contains("requerido", result.Mensaje.ToLower());
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
            Assert.Contains("paciente", result.Mensaje.ToLower());
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

            // Crear Person para Patient
            var patientPerson = new Person
            {
                PersonId = 1,
                FirstName = "Juan",
                LastName = "Pérez",
                IdentificationNumber = "001-0000001-1",
                DateOfBirth = new DateOnly(1990, 1, 1),
                Gender = "M",
                UserId = 1
            };

            var patient = new Patient
            {
                PatientId = 1,
                PhoneNumber = "809-555-1234",
                Address = "Calle Principal 123",
                EmergencyContactName = "María Pérez",
                EmergencyContactPhone = "809-555-5678",
                BloodType = "O+",
                Allergies = "Ninguna",
                InsuranceProviderId = 1,
                IsActive = true,
                CreatedAt = DateTime.Now,
                PatientNavigation = patientPerson
            };

            // Crear Person para Doctor
            var doctorPerson = new Person
            {
                PersonId = 2,
                FirstName = "Carlos",
                LastName = "Fernández",
                IdentificationNumber = "001-0000002-2",
                DateOfBirth = new DateOnly(1980, 5, 15),
                Gender = "M",
                UserId = 2
            };

            var specialty = new Specialty
            {
                SpecialtyId = 1,
                SpecialtyName = "Cardiología",
                IsActive = true
            };

            var doctor = new Doctor
            {
                DoctorId = 1,
                SpecialtyId = 1,
                LicenseNumber = "LIC001",
                LicenseExpirationDate = new DateOnly(2026, 12, 31),
                YearsOfExperience = 10,
                Education = "MD",
                IsActive = true,
                CreatedAt = DateTime.Now,
                DoctorNavigation = doctorPerson,
                Specialty = specialty
            };

            var status = new Status
            {
                StatusId = 1,
                StatusName = "Pendiente"
            };

            var appointmentResult = new Appointment
            {
                AppointmentId = 1,
                PatientId = 1,
                DoctorId = 1,
                AppointmentDate = validDate,
                StatusId = 1,
                CreatedAt = DateTime.Now,
                Patient = patient,
                Doctor = doctor,
                Status = status
            };

            // Setup mocks
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
            Assert.True(result.Exitoso, $"Expected success but got failure with message: {result.Mensaje}");
            Assert.NotNull(result.Datos);
            Assert.Equal(1, result.Datos.AppointmentId);
        }
    }
}