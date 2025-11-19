using Microsoft.EntityFrameworkCore;
using SGMC.Domain.Entities.System;
using SGMC.Domain.Repositories.System;
using SGMC.Persistence.Context;
using SGMC.Persistence.Repositories.System;

namespace SGMC.Tests.Repositories
{
    public class RoleRepositoryTests
    {
        private readonly IRoleRepository _repository;
        private readonly HealtSyncContext _context;

        public RoleRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<HealtSyncContext>()
                .UseInMemoryDatabase($"RoleDB_{Guid.NewGuid()}")
                .Options;
            _context = new HealtSyncContext(options);
            _repository = new RoleRepository(_context);
        }

        [Fact]
        public async Task GetActiveRolesAsync_ExcludesInactive()
        {
            await _context.Roles.AddRangeAsync(
                new Role { RoleName = "Admin", IsActive = true },
                new Role { RoleName = "Guest", IsActive = false }
            );
            await _context.SaveChangesAsync();

            var active = await _repository.GetActiveRolesAsync();
            Assert.Single(active);
            Assert.Equal("Admin", active.First().RoleName);
        }
    }
}