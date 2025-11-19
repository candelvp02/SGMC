using Microsoft.EntityFrameworkCore;
using SGMC.Domain.Entities.Users;
using SGMC.Persistence.Context;
using SGMC.Persistence.Repositories.Users;

namespace SGMC.Tests.Repositories
{
    public class EmployeeRepositoryTests
    {
        private HealtSyncContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<HealtSyncContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new HealtSyncContext(options);
        }

        [Fact]
        public async Task GetActiveEmployeesAsync_ShouldReturn_OnlyActiveEmployees()
        {
            // ARRANGE
            var dbName = Guid.NewGuid().ToString();
            using (var context = CreateContext(dbName))
            {
                context.Employees.AddRange(
                    new Employee { EmployeeId = 1, JobTitle = "Admin", IsActive = true, CreatedAt = DateTime.Now },
                    new Employee { EmployeeId = 2, JobTitle = "User", IsActive = false, CreatedAt = DateTime.Now },
                    new Employee { EmployeeId = 3, JobTitle = "Manager", IsActive = true, CreatedAt = DateTime.Now }
                );
                await context.SaveChangesAsync();

                var repository = new EmployeeRepository(context);

                // ACT
                var result = await repository.GetActiveEmployeesAsync();

                // ASSERT
                Assert.NotNull(result);
                Assert.Equal(2, result.Count());
                Assert.True(result.All(e => e.IsActive));
            }
        }
    }
}