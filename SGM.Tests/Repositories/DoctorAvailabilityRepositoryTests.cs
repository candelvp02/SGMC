using Microsoft.EntityFrameworkCore;
using SGMC.Domain.Entities.Appointments;
using SGMC.Domain.Repositories.Appointments;
using SGMC.Persistence.Context;
using SGMC.Persistence.Repositories.Appointments;

namespace SGMC.Tests.Repositories
{
    public class DoctorAvailabilityRepositoryTests
    {
        private readonly IDoctorAvailabilityRepository _repository;
        private readonly HealtSyncContext _context;

        public DoctorAvailabilityRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<HealtSyncContext>()
                .UseInMemoryDatabase($"AvailDB_{Guid.NewGuid()}")
                .Options;
            _context = new HealtSyncContext(options);
            _repository = new DoctorAvailabilityRepository(_context);
        }

        [Fact]
        public async Task IsAvailableAsync_ReturnsTrue_WhenSlotIsActiveAndInTime()
        {
            var avail = new DoctorAvailability
            {
                DoctorId = 1,
                AvailableDate = new DateOnly(2025, 6, 10),
                StartTime = new TimeOnly(9, 0),
                EndTime = new TimeOnly(17, 0),
                IsActive = true
            };
            await _context.DoctorAvailabilities.AddAsync(avail);
            await _context.SaveChangesAsync();

            var isAvailable = await _repository.IsAvailableAsync(1, new DateOnly(2025, 6, 10), new TimeOnly(10, 30));
            Assert.True(isAvailable);
        }

        [Fact]
        public async Task HasConflictAsync_ReturnsTrue_WhenOverlap()
        {
            await _context.DoctorAvailabilities.AddAsync(new DoctorAvailability
            {
                DoctorId = 1,
                AvailableDate = new DateOnly(2025, 6, 10),
                StartTime = new TimeOnly(9, 0),
                EndTime = new TimeOnly(11, 0),
                IsActive = true
            });
            await _context.SaveChangesAsync();

            var hasConflict = await _repository.HasConflictAsync(1, new DateOnly(2025, 6, 10), new TimeOnly(10, 30), new TimeOnly(12, 0));
            Assert.True(hasConflict);
        }
    }
}
