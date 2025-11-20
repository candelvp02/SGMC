using Microsoft.EntityFrameworkCore;
using SGMC.Domain.Entities.System;
using SGMC.Domain.Repositories.System;
using SGMC.Persistence.Context;
using SGMC.Persistence.Repositories.System;

namespace SGMC.Tests.Repositories
{
    public class NotificationRepositoryTests
    {
        private readonly INotificationRepository _repository;
        private readonly HealtSyncContext _context;

        public NotificationRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<HealtSyncContext>()
                .UseInMemoryDatabase($"NotificationDB_{Guid.NewGuid()}")
                .Options;
            _context = new HealtSyncContext(options);
            _repository = new NotificationRepository(_context);
        }

        [Fact]
        public async Task GetUnreadByUserIdAsync_ReturnsUnsent()
        {
            await _context.Notifications.AddRangeAsync(
           //     new Notification { UserId = 1, Message = "Read", SentAt = DateTime.Now },
               // new Notification { UserId = 1, Message = "Unread", SentAt = null }
            );
            await _context.SaveChangesAsync();

            var unread = await _repository.GetUnreadByUserIdAsync(1);
            Assert.Single(unread);
         //   Assert.Equal("Unread", unread.First().Message);
        }

        [Fact]
        public async Task GetUnreadCountAsync_ReturnsCorrectCount()
        {
            await _context.Notifications.AddAsync(new Notification { UserId = 2, SentAt = null });
            await _context.SaveChangesAsync();

            var count = await _repository.GetUnreadCountAsync(2);
            Assert.Equal(1, count);
        }
    }
}