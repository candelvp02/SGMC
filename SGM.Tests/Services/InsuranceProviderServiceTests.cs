using Moq;
using SGMC.Application.Dto.Insurance;
using SGMC.Application.Services;
using SGMC.Domain.Entities.Insurance;
using SGMC.Domain.Repositories.Insurance;
using Microsoft.Extensions.Logging;
using SGMC.Application.Interfaces.Service;

namespace SGMC.Tests.Services
{
    public class InsuranceProviderServiceTests
    {
        private readonly Mock<IInsuranceProviderRepository> _repoMock;
        private readonly Mock<ILogger<InsuranceProviderService>> _loggerMock;
        private readonly IInsuranceProviderService _service;
        private readonly Mock<INetworkTypeRepository> _networkRepoMock;

        public InsuranceProviderServiceTests()
        {
            _repoMock = new Mock<IInsuranceProviderRepository>();
            _loggerMock = new Mock<ILogger<InsuranceProviderService>>();
            _networkRepoMock = new Mock<INetworkTypeRepository>();
            _service = new InsuranceProviderService(
                _repoMock.Object,
                _networkRepoMock.Object,
                _loggerMock.Object);

            _networkRepoMock.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        }

        [Fact]
        public async Task CreateAsync_WhenDtoIsNull_ReturnsFailure()
        {
            var result = await _service.CreateAsync(null!);
            Assert.False(result.Exitoso);
            Assert.Equal("Los datos del proveedor son requeridos", result.Mensaje);
        }

        [Fact]
        public async Task CreateAsync_WhenNameIsEmpty_ReturnsFailure()
        {
            var dto = new CreateInsuranceProviderDto
            {
                Name = string.Empty,
                Email = "test@test.com",
                PhoneNumber = "809-555-1234",
                Address = "Fake St",
                NetworkTypeId = 1
            };
            var result = await _service.CreateAsync(dto);

            Assert.False(result.Exitoso);
            var mensajeLower = result.Mensaje.ToLower();
            Assert.True(
                mensajeLower.Contains("nombre") ||
                mensajeLower.Contains("requerido") ||
                mensajeLower.Contains("validación"),
                $"Expected message to contain 'nombre', 'requerido' or 'validación', but got: {result.Mensaje}"
            );
        }

        [Fact]
        public async Task CreateAsync_WhenValid_ReturnsSuccess()
        {
            // ARRANGE
            var dto = new CreateInsuranceProviderDto
            {
                Name = "Test Insurance",
                Email = "test@test.com",
                PhoneNumber = "809-555-1234",
                Address = "calle 7w 19 lucerna sde rd",
                NetworkTypeId = 1
            };
            var provider = new InsuranceProvider
            {
                InsuranceProviderId = 1,
                Name = "Test Insurance",
                NetworkTypeId = 1
            };

            _repoMock.Setup(r => r.AddAsync(It.IsAny<InsuranceProvider>())).ReturnsAsync(provider);
            _repoMock.Setup(r => r.GetByNameAsync(It.IsAny<string>())).ReturnsAsync((InsuranceProvider)null!);

            var result = await _service.CreateAsync(dto);

            Assert.True(result.Exitoso);
            Assert.Equal("Proveedor de seguro creado correctamente", result.Mensaje);
        }

        [Fact]
        public async Task UpdateAsync_WhenDtoIsNull_ReturnsFailure()
        {
            var result = await _service.UpdateAsync(null!);
            Assert.False(result.Exitoso);
            Assert.Equal("Los datos del proveedor son requeridos", result.Mensaje);
        }

        [Fact]
        public async Task UpdateAsync_WhenIdIsInvalid_ReturnsFailure()
        {
            var dto = new UpdateInsuranceProviderDto { InsuranceProviderId = -1, Name = "Test", NetworkTypeId = 1 };
            var result = await _service.UpdateAsync(dto);

            Assert.False(result.Exitoso);
            var mensajeLower = result.Mensaje.ToLower();
            Assert.True(
                mensajeLower.Contains("id") ||
                mensajeLower.Contains("inválido") ||
                mensajeLower.Contains("validación"),
                $"Expected message to contain 'id', 'inválido' or 'validación', but got: {result.Mensaje}"
            );
        }

        [Fact]
        public async Task UpdateAsync_WhenNameIsEmpty_ReturnsFailure()
        {
            // ARRANGE
            var existing = new InsuranceProvider
            {
                InsuranceProviderId = 1,
                Name = "Old Name",
                NetworkTypeId = 1,
                IsActive = true
            };

            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
            _networkRepoMock.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);

            // DTO con nombre vacío pero NetworkTypeId válido
            var dto = new UpdateInsuranceProviderDto
            {
                InsuranceProviderId = 1,
                Name = string.Empty,
                NetworkTypeId = 1,
                PhoneNumber = "809-555-1234",
                Email = "test@test.com",
                Address = "Test Address"
            };

            // ACT
            var result = await _service.UpdateAsync(dto);

            // ASSERT
            Assert.False(result.Exitoso, $"Expected failure but got success with message: {result.Mensaje}");

            var mensajeLower = result.Mensaje.ToLower();
            Assert.True(
                mensajeLower.Contains("nombre") ||
                mensajeLower.Contains("requerido") ||
                mensajeLower.Contains("validación") ||
                mensajeLower.Contains("vacío") ||
                mensajeLower.Contains("inválido"),
                $"Expected message to contain validation error about 'nombre', but got: {result.Mensaje}"
            );
        }

        [Fact]
        public async Task UpdateAsync_WhenProviderNotFound_ReturnsFailure()
        {
            var dto = new UpdateInsuranceProviderDto
            {
                InsuranceProviderId = 1,
                Name = "Updated",
                NetworkTypeId = 1
            };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((InsuranceProvider?)null);

            var result = await _service.UpdateAsync(dto);
            Assert.False(result.Exitoso);
            Assert.Equal("Proveedor de seguro no encontrado", result.Mensaje);
        }

        [Fact]
        public async Task UpdateAsync_WhenValid_ReturnsSuccess()
        {
            // ARRANGE
            var existing = new InsuranceProvider
            {
                InsuranceProviderId = 1,
                Name = "Old Name",
                PhoneNumber = "809-111-2222",
                Email = "old@test.com",
                Address = "Old Address",
                NetworkTypeId = 1,
                IsActive = true
            };

            var dto = new UpdateInsuranceProviderDto
            {
                InsuranceProviderId = 1,
                Name = "New Name",
                PhoneNumber = "809-555-1234",
                Email = "new@test.com",
                Address = "New Address",
                IsActive = true,
                NetworkTypeId = 1
            };

            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
            _repoMock.Setup(r => r.UpdateAsync(It.IsAny<InsuranceProvider>())).Returns(Task.CompletedTask);
            _networkRepoMock.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);

            // ACT
            var result = await _service.UpdateAsync(dto);

            // ASSERT
            Assert.True(result.Exitoso);
            Assert.Equal("Proveedor de seguro actualizado correctamente", result.Mensaje);
        }

        [Fact]
        public async Task GetByIdAsync_WhenIdIsInvalid_ReturnsFailure()
        {
            var result = await _service.GetByIdAsync(-1);
            Assert.False(result.Exitoso);
            Assert.Equal("El ID del proveedor es inválido", result.Mensaje);
        }

        [Fact]
        public async Task GetByIdAsync_WhenProviderNotFound_ReturnsFailure()
        {
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((InsuranceProvider?)null);
            var result = await _service.GetByIdAsync(1);
            Assert.False(result.Exitoso);
            Assert.Equal("Proveedor de seguro no encontrado", result.Mensaje);
        }

        [Fact]
        public async Task GetByIdAsync_WhenValid_ReturnsSuccess()
        {
            var provider = new InsuranceProvider { InsuranceProviderId = 1, Name = "Test Provider" };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(provider);

            var result = await _service.GetByIdAsync(1);

            Assert.True(result.Exitoso);
            Assert.Equal("Proveedor de seguro obtenido correctamente", result.Mensaje);
        }

        [Fact]
        public async Task ExistsAsync_WhenIdIsInvalid_ReturnsFalseWithSuccess()
        {
            var result = await _service.ExistsAsync(-1);
            Assert.True(result.Exitoso);
            Assert.False(result.Datos);
            Assert.Equal("ID inválido", result.Mensaje);
        }

        [Fact]
        public async Task ExistsAsync_WhenProviderExists_ReturnsTrue()
        {
            _repoMock.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
            var result = await _service.ExistsAsync(1);
            Assert.True(result.Exitoso);
            Assert.True(result.Datos);
        }

        [Fact]
        public async Task ExistsAsync_WhenProviderDoesNotExist_ReturnsFalse()
        {
            _repoMock.Setup(r => r.ExistsAsync(999)).ReturnsAsync(false);
            var result = await _service.ExistsAsync(999);
            Assert.True(result.Exitoso);
            Assert.False(result.Datos);
        }

        [Fact]
        public async Task GetActiveAsync_WhenNoProviders_ReturnsEmptyList()
        {
            _repoMock.Setup(r => r.GetActiveProviderAsync()).ReturnsAsync(new List<InsuranceProvider>());
            var result = await _service.GetActiveAsync();
            Assert.True(result.Exitoso);
            Assert.NotNull(result.Datos);
            Assert.Empty(result.Datos);
        }

        [Fact]
        public async Task GetActiveAsync_WhenProvidersExist_ReturnsList()
        {
            List<InsuranceProvider> providers = [new() { InsuranceProviderId = 1, Name = "Active1" }];
            _repoMock.Setup(r => r.GetActiveProviderAsync()).ReturnsAsync(providers);

            var result = await _service.GetActiveAsync();

            Assert.True(result.Exitoso);
            Assert.NotNull(result.Datos);
            Assert.Equal(1, result.Datos?.Count);
        }

        [Fact]
        public async Task GetAllAsync_WhenNoProviders_ReturnsEmptyList()
        {
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<InsuranceProvider>());
            var result = await _service.GetAllAsync();
            Assert.True(result.Exitoso);
            Assert.NotNull(result.Datos);
            Assert.Empty(result.Datos);
        }

        [Fact]
        public async Task GetAllAsync_WhenProvidersExist_ReturnsList()
        {
            List<InsuranceProvider> providers = [new() { InsuranceProviderId = 1, Name = "P1" }];
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(providers);

            var result = await _service.GetAllAsync();

            Assert.True(result.Exitoso);
            Assert.NotNull(result.Datos);
            Assert.Equal(1, result.Datos?.Count);
        }

        [Fact]
        public async Task DeleteAsync_WhenProviderNotFound_ReturnsFailure()
        {
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((InsuranceProvider?)null);
            var result = await _service.DeleteAsync(1);
            Assert.False(result.Exitoso);
            Assert.Equal("Proveedor de seguro no encontrado", result.Mensaje);
        }

        [Fact]
        public async Task DeleteAsync_WhenValid_ReturnsSuccess()
        {
            var provider = new InsuranceProvider { InsuranceProviderId = 1, Name = "Test Provider" };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(provider);
            _repoMock.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

            var result = await _service.DeleteAsync(1);

            Assert.True(result.Exitoso);
            Assert.Equal("Proveedor de seguro eliminado correctamente", result.Mensaje);
        }
    }
}