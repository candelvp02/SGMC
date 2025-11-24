using Moq;
using SGMC.Application.Dto.Medical;
using SGMC.Application.Interfaces.Service;
using SGMC.Application.Services;
using SGMC.Domain.Entities.Medical;
using SGMC.Domain.Repositories.Medical;
using Microsoft.Extensions.Logging;

namespace SGMC.Tests.Services
{
    public class SpecialtyServiceTests
    {
        private readonly Mock<ISpecialtyRepository> _repoMock;
        private readonly Mock<ILogger<SpecialtyService>> _loggerMock;
        private readonly ISpecialtyService _service;

        public SpecialtyServiceTests()
        {
            _repoMock = new Mock<ISpecialtyRepository>();
            _loggerMock = new Mock<ILogger<SpecialtyService>>();
            _service = new SpecialtyService(_repoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task CreateAsync_WhenNameExists_ReturnsFailure()
        {
            var dto = new CreateSpecialtyDto { SpecialtyName = "Cardiology" };
            _repoMock.Setup(r => r.ExistsByNameAsync("Cardiology")).ReturnsAsync(true);

            var result = await _service.CreateAsync(dto);

            Assert.False(result.Exitoso);
            var mensajeLower = result.Mensaje.ToLower();
            Assert.True(
                mensajeLower.Contains("existe") ||
                mensajeLower.Contains("ya existe") ||
                mensajeLower.Contains("duplicad"),
                $"Expected message to contain 'existe', 'ya existe' or 'duplicad', but got: {result.Mensaje}"
            );
        }

        [Fact]
        public async Task CreateAsync_WhenValid_ReturnsSuccess()
        {
            var dto = new CreateSpecialtyDto { SpecialtyName = "Neurology" };
            _repoMock.Setup(r => r.ExistsByNameAsync("Neurology")).ReturnsAsync(false);
            _repoMock.Setup(r => r.AddAsync(It.IsAny<Specialty>()))
                .ReturnsAsync(new Specialty { SpecialtyId = 1, SpecialtyName = "Neurology" });

            var result = await _service.CreateAsync(dto);

            Assert.True(result.Exitoso);
            var mensajeLower = result.Mensaje.ToLower();
            Assert.True(
                mensajeLower.Contains("correctamente") ||
                mensajeLower.Contains("éxito") ||
                mensajeLower.Contains("cread"),
                $"Expected message to contain 'correctamente', 'éxito' or 'cread', but got: {result.Mensaje}"
            );
        }
    }
}
