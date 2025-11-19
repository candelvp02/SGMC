using Microsoft.EntityFrameworkCore;
using SGMC.Domain.Entities.Users;
using SGMC.Domain.Repositories.Users;
using SGMC.Persistence.Context;
using SGMC.Persistence.Repositories.Users;
using SGMC.Domain.Entities.Medical;

namespace SGMC.Tests.Repositories
{
    public class DoctorRepositoryTests
    {
        private readonly IDoctorRepository _repository;
        private readonly HealtSyncContext _context;

        public DoctorRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<HealtSyncContext>()
                .UseInMemoryDatabase($"DoctorDB_{Guid.NewGuid()}")
                .Options;
            _context = new HealtSyncContext(options);
            _repository = new DoctorRepository(_context);
        }

        [Fact]
        public async Task GetBySpecialtyIdAsync_ReturnsActiveOnly()
        {
            // ARRANGE
            var specialty = new Specialty { SpecialtyId = 5, SpecialtyName = "TestSpec" };
            var user1 = new User { UserId = 1, Email = "d1@test.com", PasswordHash = "hash1", RoleId = 2, IsActive = true, CreatedAt = DateTime.Now };
            var user2 = new User { UserId = 2, Email = "d2@test.com", PasswordHash = "hash2", RoleId = 2, IsActive = true, CreatedAt = DateTime.Now };
            var person1 = new Person { PersonId = 1, User = user1, FirstName = "D1", LastName = "Test" };
            var person2 = new Person { PersonId = 2, User = user2, FirstName = "D2", LastName = "Test" };

            await _context.Specialties.AddAsync(specialty);
            await _context.Users.AddRangeAsync(user1, user2);
            await _context.Persons.AddRangeAsync(person1, person2);
            await _context.SaveChangesAsync();

            await _context.Doctors.AddRangeAsync(
                new Doctor
                {
                    DoctorId = 1,
                    SpecialtyId = 5,
                    IsActive = true,
                    LicenseNumber = "L1",
                    CreatedAt = DateTime.Now,
                    Specialty = specialty,
                    DoctorNavigation = person1
                },
                new Doctor
                {
                    DoctorId = 2,
                    SpecialtyId = 5,
                    IsActive = false,
                    LicenseNumber = "L2",
                    CreatedAt = DateTime.Now,
                    Specialty = specialty,
                    DoctorNavigation = person2
                }
            );
            await _context.SaveChangesAsync();
            _context.Entry(specialty).State = EntityState.Detached;

            // ACT
            var doctors = await _repository.GetBySpecialtyIdAsync(5);

            // ASSERT
            Assert.Single(doctors);
            Assert.Equal("L1", doctors.First().LicenseNumber);
        }

        [Fact]
        public async Task ExistsByLicenseNumberAsync_ReturnsTrue()
        {
            await _context.Doctors.AddAsync(new Doctor { DoctorId = 1, LicenseNumber = "ABC123", IsActive = true, SpecialtyId = 1, CreatedAt = DateTime.Now });
            await _context.SaveChangesAsync();

            var exists = await _repository.ExistsByLicenseNumberAsync("ABC123");
            Assert.True(exists);
        }
    }
}