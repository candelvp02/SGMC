using Microsoft.EntityFrameworkCore;
using SGMC.Domain.Entities.Insurance;
using SGMC.Domain.Repositories.Insurance;
using SGMC.Persistence.Context;
using SGMC.Persistence.Repositories.Insurance;


namespace SGMC.Tests.Repositories
{
    public class InsuranceProviderRepositoryTests
    {
        private readonly IInsuranceProviderRepository _repository;
        private readonly HealtSyncContext _context;

        public InsuranceProviderRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<HealtSyncContext>()
                .UseInMemoryDatabase($"InsuranceDB_{Guid.NewGuid()}")
                .Options;
            _context = new HealtSyncContext(options);
            _repository = new InsuranceProviderRepository(_context);
        }

        [Fact]
        public async Task GetActiveProviderAsync_ExcludesInactive()
        {
            await _context.InsuranceProviders.AddRangeAsync(
                new InsuranceProvider { Name = "Active", IsActive = true },
                new InsuranceProvider { Name = "Inactive", IsActive = false }
            );
            await _context.SaveChangesAsync();

            var active = await _repository.GetActiveProviderAsync();
            Assert.Single(active);
            Assert.Equal("Active", active.First().Name);
        }

        [Fact]
        public async Task GetByNameAsync_FindsByName()
        {
            await _context.InsuranceProviders.AddAsync(new InsuranceProvider { Name = "TestIns", IsActive = true });
            await _context.SaveChangesAsync();

            var result = await _repository.GetByNameAsync("TestIns");
            Assert.NotNull(result);
        }
    }
}