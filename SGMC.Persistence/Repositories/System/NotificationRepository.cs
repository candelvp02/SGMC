using Microsoft.EntityFrameworkCore;
using SGMC.Domain.Entities.System;
using SGMC.Domain.Repositories.System;
using SGMC.Persistence.Base;
using SGMC.Persistence.Context;

namespace SGMC.Persistence.Repositories.System
{
    public sealed class NotificationRepository : BaseRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(HealtSyncContext context) : base(context) { }

        public async Task<IEnumerable<Notification>> GetByUserIdAsync(int userId)
            => await _dbSet.Where(n => n.UserId == userId).ToListAsync();

        public async Task<IEnumerable<Notification>> GetRecentByUserIdAsync(int userId, int count = 10)
        {
            return await _dbSet
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.SentAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetUnreadByUserIdAsync(int userId)
            => await _dbSet.Where(n => n.UserId == userId && n.SentAt == null).ToListAsync();

        public async Task<int> GetUnreadCountAsync(int userId)
            => await _dbSet.CountAsync(n => n.UserId == userId && n.SentAt == null);

        Task INotificationRepository.DeleteAsync(int notificationId)
        {
            return DeleteAsync(notificationId);
        }

        public async Task<IEnumerable<Notification>> GetPendingByUserIdAsync(int userId)
        {
            return await _dbSet
                .Where(n => n.UserId == userId && n.SentAt == null)
                .ToListAsync();
        }
    }
}