using SGMC.Domain.Entities.Users;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGMC.Domain.Entities.System
{
    [Table("Notifications", Schema = "system")]

    public partial class Notification
    {

        public int NotificationId { get; set; }

        public int UserId { get; set; }

        public DateTime? SentAt { get; set; }

        public virtual User? User { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int RecipientId { get; set; }
        public string? Title { get; set; }
        public bool IsRead { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}