using Microsoft.EntityFrameworkCore;
using SGMC.Domain.Entities.Medical;
using SGMC.Domain.Repositories.Medical;
using SGMC.Persistence.Context;
using SGMC.Persistence.Repositories.Medical;

namespace SGMC.Tests.Repositories
{
    public class SpecialtyRepositoryTests
    {
        private readonly ISpecialtyRepository _repository;
        private readonly HealtSyncContext _context;

        public SpecialtyRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<HealtSyncContext>()
                .UseInMemoryDatabase($"SpecialtyDB_{Guid.NewGuid()}")
                .Options;
            _context = new HealtSyncContext(options);
            _repository = new SpecialtyRepository(_context);
        }

        [Fact]
        public async Task GetActiveSpecialtiesAsync_ExcludesInactive()
        {
            await _context.Specialties.AddRangeAsync(
                new Specialty { SpecialtyName = "Cardiology", IsActive = true },
                new Specialty { SpecialtyName = "Inactive", IsActive = false }
            );
            await _context.SaveChangesAsync();

            var active = await _repository.GetActiveSpecialtiesAsync();
            Assert.Single(active);
            Assert.Equal("Cardiology", active.First().SpecialtyName);
        }

        [Fact]
        public async Task ExistsByNameAsync_ReturnsTrue()
        {
            await _context.Specialties.AddAsync(new Specialty { SpecialtyName = "Dermatology", IsActive = true });
            await _context.SaveChangesAsync();

            var exists = await _repository.ExistsByNameAsync("Dermatology");
            Assert.True(exists);
        }
    }
}