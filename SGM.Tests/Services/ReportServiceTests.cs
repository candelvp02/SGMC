using Moq;
using SGMC.Application.Dto.Appointments;
using SGMC.Application.Interfaces.Service;
using SGMC.Application.Services;
using SGMC.Domain.Entities.Appointments;
using SGMC.Domain.Repositories.Appointments;
using SGMC.Domain.Repositories.Users;
using Microsoft.Extensions.Logging;

namespace SGMC.Tests.Services
{
    public class ReportServiceTests
    {
        private readonly Mock<IAppointmentRepository> _apptRepoMock;
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<ILogger<ReportService>> _loggerMock;
        private readonly IReportService _service;

        public ReportServiceTests()
        {
            _apptRepoMock = new Mock<IAppointmentRepository>();
            _userRepoMock = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILogger<ReportService>>();
            _service = new ReportService(_apptRepoMock.Object, _userRepoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetAppointmentStatisticsAsync_ReturnsStats()
        {
            var appointments = new List<Appointment>
            {
                new Appointment { StatusId = 1 }, // pending
                new Appointment { StatusId = 2 }, // confirmed
                new Appointment { StatusId = 3 }  // cancelled
            };
            _apptRepoMock.Setup(r => r.GetByDateRangeAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(appointments);

            var result = await _service.GetAppointmentStatisticsAsync(new ReportFilterDto());
            Assert.True(result.Exitoso);
            Assert.Equal(3, result.Datos?.TotalAppointments);
            Assert.Equal(1, result.Datos?.ConfirmedAppointments);
            Assert.Equal(1, result.Datos?.CancelledAppointments);
        }
    }
}