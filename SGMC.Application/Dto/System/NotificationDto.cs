namespace SGMC.Application.Dto.System
{
    public class NotificationDto
    {
        public int NotificationId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime? SentAt { get; set; }
        public int RecipientId { get; internal set; }
        public string? Title { get; internal set; }
        public object? UpdatedAt { get; internal set; }
        public bool IsRead { get; internal set; }
        public DateTime CreatedAt { get; internal set; }
    }
    public class CreateNotificationDto
    {
        public int UserId { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}