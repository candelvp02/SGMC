using Microsoft.Extensions.Logging;
using Moq;
using SGMC.Application.Interfaces.Service;
using SGMC.Application.Services;
using SGMC.Domain.Repositories.Insurance;
using SGMC.Domain.Repositories.Users;

namespace SGMC.Tests.Services
{
    public class PatientServiceTests
    {
        private readonly Mock<IPatientRepository> _repoMock;
        private readonly Mock<ILogger<PatientService>> _loggerMock;
        private readonly IPatientService _service;
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<IPersonRepository> _personRepoMock;
        private readonly Mock<IInsuranceProviderRepository> _insuranceProviderRepoMock;

        public PatientServiceTests()
        {
            _repoMock = new Mock<IPatientRepository>();
            _loggerMock = new Mock<ILogger<PatientService>>();
            _userRepoMock = new Mock<IUserRepository>();
            _personRepoMock = new Mock<IPersonRepository>();
            _insuranceProviderRepoMock = new Mock<IInsuranceProviderRepository>();

            _service = new PatientService(
                _repoMock.Object,
                _loggerMock.Object,
                _userRepoMock.Object,
                _personRepoMock.Object,
                _insuranceProviderRepoMock.Object);
        }

        [Fact]
        public async Task CreateAsync_WhenDtoIsNull_ReturnsFailure()
        {
            var result = await _service.CreateAsync(null!);
            Assert.False(result.Exitoso);
            Assert.Contains("datos del paciente son requeridos", result.Mensaje.ToLower());
        }
    }
}