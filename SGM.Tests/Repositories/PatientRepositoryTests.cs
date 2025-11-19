using Microsoft.EntityFrameworkCore;
using SGMC.Domain.Entities.Insurance;
using SGMC.Domain.Entities.Users;
using SGMC.Domain.Repositories.Users;
using SGMC.Persistence.Context;
using SGMC.Persistence.Repositories.Users;


namespace SGMC.Tests.Repositories
{
    public class PatientRepositoryTests
    {
        private readonly IPatientRepository _repository;
        private readonly HealtSyncContext _context;

        public PatientRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<HealtSyncContext>()
                .UseInMemoryDatabase($"PatientDB_{Guid.NewGuid()}")
                .Options;
            _context = new HealtSyncContext(options);
            _repository = new PatientRepository(_context);
        }

        [Fact]
        public async Task GetByIdWithDetailsAsync_IncludesPersonAndInsurance()
        {
            // ARRANGE
            var user = new User { UserId = 1, Email = "test@user.com", PasswordHash = "hash" };
            var person = new Person { PersonId = 1, IdentificationNumber = "ID123", User = user };
            var insurance = new InsuranceProvider { InsuranceProviderId = 1, Name = "TestIns" };
            var patient = new Patient
            {
                PatientId = 1,
                InsuranceProviderId = 1,
                IsActive = true,
                PhoneNumber = "123",
                CreatedAt = DateTime.Now,
                PatientNavigation = person,
                InsuranceProvider = insurance
            };

            _context.Users.Add(user);
            _context.Persons.Add(person);
            _context.InsuranceProviders.Add(insurance);
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
            _context.Entry(patient).State = EntityState.Detached;

            // ACT
            var result = await _repository.GetByIdWithDetailsAsync(1);

            // ASSERT
            Assert.NotNull(result);
            Assert.NotNull(result.PatientNavigation);
            Assert.NotNull(result.InsuranceProvider);
            Assert.Equal("ID123", result.PatientNavigation.IdentificationNumber);
            Assert.Equal("TestIns", result.InsuranceProvider.Name);
        }
    }
}
