using Microsoft.EntityFrameworkCore;
using SGMC.Domain.Entities.Medical;
using SGMC.Domain.Repositories.Medical;
using SGMC.Persistence.Context;
using SGMC.Persistence.Repositories.Medical;

namespace SGMC.Tests.Repositories
{
    public class AvailabilityModeRepositoryTests
    {
        private readonly IAvailabilityModeRepository _repository;
        private readonly HealtSyncContext _context;

        public AvailabilityModeRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<HealtSyncContext>()
                .UseInMemoryDatabase($"AvailModeDB_{Guid.NewGuid()}")
                .Options;
            _context = new HealtSyncContext(options);
            _repository = new AvailabilityModeRepository(_context);
        }

        [Fact]
        public async Task GetActiveModesAsync_ExcludesInactive()
        {
            await _context.AvailabilityModes.AddRangeAsync(
                new AvailabilityMode { AvailabilityMode1 = "Online", IsActive = true },
                new AvailabilityMode { AvailabilityMode1 = "Offline", IsActive = false }
            );
            await _context.SaveChangesAsync();

            var active = await _repository.GetActiveModesAsync();
            Assert.Single(active);
            Assert.Equal("Online", active.First().AvailabilityMode1);
        }
    }
}