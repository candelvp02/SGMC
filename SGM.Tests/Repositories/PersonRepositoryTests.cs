using Microsoft.EntityFrameworkCore;
using SGMC.Domain.Entities.Users;
using SGMC.Domain.Repositories.Users;
using SGMC.Persistence.Context;
using SGMC.Persistence.Repositories.Users;

namespace SGMC.Tests.Repositories
{
    public class PersonRepositoryTests
    {
        private readonly IPersonRepository _repository;
        private readonly HealtSyncContext _context;

        public PersonRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<HealtSyncContext>()
                .UseInMemoryDatabase($"PersonDB_{Guid.NewGuid()}")
                .Options;
            _context = new HealtSyncContext(options);
            _repository = new PersonRepository(_context);
        }

        [Fact]
        public async Task GetByIdentificationNumberAsync_FindsPerson()
        {
            // ARRANGE
            var user = new User { UserId = 1, Email = "test@test.com", PasswordHash = "hash" };
            var person = new Person { PersonId = 1, IdentificationNumber = "ID789", FirstName = "John", User = user };

            await _context.Users.AddAsync(user);
            await _context.Persons.AddAsync(person);
            await _context.SaveChangesAsync();

            // ACT
            var result = await _repository.GetByIdentificationNumberAsync("ID789");

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal("John", result.FirstName);
        }
    }
}