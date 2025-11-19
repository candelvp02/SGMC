using Microsoft.EntityFrameworkCore;
using SGMC.Domain.Entities.Insurance;
using SGMC.Domain.Repositories.Insurance;
using SGMC.Persistence.Context;
using SGMC.Persistence.Repositories.Insurance;

namespace SGMC.Tests.Repositories
{
    public class NetworkTypeRepositoryTest
    {
        private readonly INetworkTypeRepository _repository;
        private readonly HealtSyncContext _context;

        public NetworkTypeRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<HealtSyncContext>()
                .UseInMemoryDatabase(databaseName: "HealtSyncDb_NetworkTypeTest")
                .Options;

            _context = new HealtSyncContext(options);
            _repository = new NetworkTypeRepository(_context);

            SeedData();
        }

        private void SeedData()
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            var networkTypes = new List<NetworkType>
            {
                new NetworkType { NetworkTypeId = 1, Name = "PPO", IsActive = true, CreatedAt = DateTime.Now },
                new NetworkType { NetworkTypeId = 2, Name = "HMO", IsActive = true, CreatedAt = DateTime.Now },
                new NetworkType { NetworkTypeId = 3, Name = "Obsoleto", IsActive = false, CreatedAt = DateTime.Now }
            };
            _context.NetworkTypes.AddRange(networkTypes);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetActiveNetworkTypesAsync_ShouldReturn_OnlyActiveTypes()
        {
            var result = await _repository.GetActiveNetworkTypesAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.True(result.All(n => n!.IsActive));
        }

        [Fact]
        public async Task GetByNameAsync_ShouldReturn_CorrectType()
        {
            var nameToFind = "HMO";

            var result = await _repository.GetByNameAsync(nameToFind);

            Assert.NotNull(result);
            Assert.Equal(2, result.NetworkTypeId);
            Assert.Equal(nameToFind, result.Name);
        }
    }
}