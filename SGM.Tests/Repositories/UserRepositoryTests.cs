using Microsoft.EntityFrameworkCore;
using SGMC.Domain.Entities.System;
using SGMC.Domain.Entities.Users;
using SGMC.Persistence.Context;
using SGMC.Persistence.Repositories.Users;

namespace SGMC.Tests.Repositories
{
    public class UserRepositoryTests
    {
        private HealtSyncContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<HealtSyncContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new HealtSyncContext(options);
        }

        [Fact]
        public async Task GetByEmailWithDetailsAsync_ShouldReturn_UserWithDetails()
        {
            // ARRANGE
            var dbName = Guid.NewGuid().ToString();
            var emailToFind = "test@user.com";

            using (var context = CreateContext(dbName))
            {
                var role = new Role { RoleId = 1, RoleName = "TestRole" };
                var person = new Person { PersonId = 1, FirstName = "Test" };
                var user = new User
                {
                    UserId = 1,
                    Email = emailToFind,
                    RoleId = 1,
                    UserNavigation = person,
                    Role = role
                };

                context.Roles.Add(role);
                context.Persons.Add(person);
                context.Users.Add(user);
                await context.SaveChangesAsync();

                var repository = new UserRepository(context);

                // ACT
                var result = await repository.GetByEmailWithDetailsAsync(emailToFind);

                // ASSERT
                Assert.NotNull(result);
                Assert.Equal(emailToFind, result.Email);
                Assert.NotNull(result.Role);
                Assert.NotNull(result.UserNavigation);
            }
        }
    }
}