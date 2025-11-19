using Microsoft.EntityFrameworkCore;
using SGMC.Domain.Entities.System;
using SGMC.Domain.Repositories.System;
using SGMC.Persistence.Context;
using SGMC.Persistence.Repositories.System;

namespace SGMC.Tests.Repositories
{
    public class StatusRepositoryTests
    {
        private readonly IStatusRepository _repository;
        private readonly HealtSyncContext _context;

        public StatusRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<HealtSyncContext>()
                .UseInMemoryDatabase($"StatusDB_{Guid.NewGuid()}")
                .Options;
            _context = new HealtSyncContext(options);
            _repository = new StatusRepository(_context);
        }

        [Fact]
        public async Task GetByNameAsync_FindsStatus()
        {
            await _context.Statuses.AddAsync(new Status { StatusName = "Completed" });
            await _context.SaveChangesAsync();

            var result = await _repository.GetByNameAsync("Completed");
            Assert.NotNull(result);
            Assert.Equal("Completed", result.StatusName);
        }
    }
}