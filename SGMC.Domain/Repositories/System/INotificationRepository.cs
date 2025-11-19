using SGMC.Domain.Entities.System;

namespace SGMC.Domain.Repositories.System
{
    public interface INotificationRepository
    {
        Task<Notification?> GetByIdAsync(int notificationId);
        Task<IEnumerable<Notification>> GetAllAsync();
        Task<Notification> AddAsync(Notification notification);
        Task UpdateAsync(Notification notification);
        Task DeleteAsync(int notificationId);
        Task<IEnumerable<Notification>> GetByUserIdAsync(int userId);
        Task<IEnumerable<Notification>> GetRecentByUserIdAsync(int userId, int count = 10);
        Task<IEnumerable<Notification>> GetUnreadByUserIdAsync(int userId);
        Task<int> GetUnreadCountAsync(int userId);
        Task<IEnumerable<Notification>> GetPendingByUserIdAsync(int userId);
    }
}